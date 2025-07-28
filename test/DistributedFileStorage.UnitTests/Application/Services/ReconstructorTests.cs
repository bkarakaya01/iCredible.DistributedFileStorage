using DistributedFileStorage.Application.Services;
using DistributedFileStorage.Application.Utils;
using DistributedFileStorage.Domain.Entities;
using DistributedFileStorage.Domain.Interfaces;
using DistributedFileStorage.Domain.Interfaces.Storage;
using DistributedFileStorage.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Text;

namespace DistributedFileStorage.UnitTests.Application.Services;

public class ReconstructorTests
{
    [Fact]
    public async Task ReconstructFileAsync_ShouldReconstructCorrectly_WhenValidMetadataExists()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var content1 = Encoding.UTF8.GetBytes("Hello ");
        var content2 = Encoding.UTF8.GetBytes("World!");
        var allContent = content1.Concat(content2).ToArray();
        var expectedChecksum = ChecksumHelper.CalculateChunkId(allContent);

        var chunk1 = new ChunkMetadata
        {
            ChunkId = ChecksumHelper.CalculateChunkId(content1),
            Order = 0,
            Size = content1.Length,
            StorageProviderName = "Mock"
        };

        var chunk2 = new ChunkMetadata
        {
            ChunkId = ChecksumHelper.CalculateChunkId(content2),
            Order = 1,
            Size = content2.Length,
            StorageProviderName = "Mock"
        };

        var fileMetadata = new FileMetadata
        {
            Id = fileId,
            FileName = "test.txt",
            FileSize = allContent.Length,
            OriginalChecksum = ChecksumHelper.CalculateChunkId(allContent),
            Chunks = [chunk1, chunk2]
        };

        var dbOptions = new DbContextOptionsBuilder<MetadataDbContext>()
            .UseInMemoryDatabase(databaseName: "ReconstructorTestDb")
            .Options;

        using var dbContext = new MetadataDbContext(dbOptions);
        dbContext.Files.Add(fileMetadata);
        dbContext.SaveChanges();

        var providerMock = new Mock<IStorageProvider>();
        providerMock.Setup(p => p.Name).Returns("Mock");
        providerMock.Setup(p => p.ReadChunkAsync(chunk1.ChunkId)).ReturnsAsync(content1);
        providerMock.Setup(p => p.ReadChunkAsync(chunk2.ChunkId)).ReturnsAsync(content2);

        var factoryMock = new Mock<IStorageProviderFactory>();
        factoryMock.Setup(f => f.GetProviderByName("Mock")).Returns(providerMock.Object);

        var outputPath = Path.Combine(Path.GetTempPath(), $"reconstructed_{Guid.NewGuid()}.txt");

        var reconstructor = new Reconstructor(factoryMock.Object, dbContext);

        // Act
        await reconstructor.ReconstructFileAsync(fileId, outputPath);

        // Assert
        Assert.True(File.Exists(outputPath));
        var fileData = await File.ReadAllBytesAsync(outputPath);
        var fileChecksum = ChecksumHelper.CalculateChunkId(fileData);
        Assert.Equal(expectedChecksum, fileChecksum);

        // Cleanup
        File.Delete(outputPath);
    }

    [Fact]
    public async Task ReconstructFileAsync_ShouldThrowInvalidOperation_WhenFileDoesNotExist()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MetadataDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new MetadataDbContext(options);
        var factoryMock = new Mock<IStorageProviderFactory>();
        var reconstructor = new Reconstructor(factoryMock.Object, dbContext);
        var nonexistentId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            reconstructor.ReconstructFileAsync(nonexistentId, "dummy.txt"));
    }

    [Fact]
    public async Task ReconstructFileAsync_ShouldThrowInvalidOperation_WhenChecksumMismatch()
    {
        // Arrange
        var fileId = Guid.NewGuid();
        var content = Encoding.UTF8.GetBytes("Mismatch!");

        var chunk = new ChunkMetadata
        {
            ChunkId = ChecksumHelper.CalculateChunkId(content),
            Order = 0,
            Size = content.Length,
            StorageProviderName = "Mock"
        };

        var fileMetadata = new FileMetadata
        {
            Id = fileId,
            FileName = "file.txt",
            FileSize = content.Length,
            OriginalChecksum = "WRONGCHECKSUM",
            Chunks = [chunk]
        };

        var options = new DbContextOptionsBuilder<MetadataDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var dbContext = new MetadataDbContext(options);
        dbContext.Files.Add(fileMetadata);
        dbContext.SaveChanges();

        var providerMock = new Mock<IStorageProvider>();
        providerMock.Setup(p => p.Name).Returns("Mock");
        providerMock.Setup(p => p.ReadChunkAsync(chunk.ChunkId)).ReturnsAsync(content);

        var factoryMock = new Mock<IStorageProviderFactory>();
        factoryMock.Setup(f => f.GetProviderByName("Mock")).Returns(providerMock.Object);

        var reconstructor = new Reconstructor(factoryMock.Object, dbContext);
        var outputPath = Path.Combine(Path.GetTempPath(), $"output_{Guid.NewGuid()}.bin");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            reconstructor.ReconstructFileAsync(fileId, outputPath));

        if (File.Exists(outputPath))
            File.Delete(outputPath);
    }
}

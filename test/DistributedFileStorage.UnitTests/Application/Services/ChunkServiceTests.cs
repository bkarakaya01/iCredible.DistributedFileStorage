using DistributedFileStorage.Application.Services;
using DistributedFileStorage.Domain.Interfaces;
using DistributedFileStorage.Domain.Interfaces.Storage;
using DistributedFileStorage.Domain.Interfaces.Strategies;
using DistributedFileStorage.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DistributedFileStorage.UnitTests.Application.Services;
public class ChunkServiceTests
{
    private readonly Mock<IStorageProviderFactory> _providerFactoryMock;
    private readonly Mock<IChunkingStrategy> _strategyMock;
    private readonly MetadataDbContext _dbContext;
    private readonly ChunkService _chunkService;

    public ChunkServiceTests()
    {
        _providerFactoryMock = new();
        _strategyMock = new();

        var options = new DbContextOptionsBuilder<MetadataDbContext>()
            .UseInMemoryDatabase("TestDb")
            .Options;

        _dbContext = new MetadataDbContext(options);
        _chunkService = new ChunkService(_providerFactoryMock.Object, _strategyMock.Object, _dbContext);
    }

    [Fact]
    public async Task ChunkAndStoreAsync_ShouldStoreChunksAndMetadata()
    {
        // Arrange
        var filePath = "test.pdf";
        var randomBytes = new byte[5 * 1024 * 1024]; // 5 MB
        new Random().NextBytes(randomBytes);        // Randomize the content
        File.WriteAllBytes(filePath, randomBytes);

        _strategyMock.Setup(x => x.GetChunkSize(It.IsAny<long>())).Returns(1024 * 1024);

        var providerMock = new Mock<IStorageProvider>();
        providerMock.Setup(x => x.Name).Returns("FakeStorage");
        providerMock.Setup(x => x.SaveChunkAsync(It.IsAny<string>(), It.IsAny<byte[]>())).Returns(Task.CompletedTask);

        _providerFactoryMock.Setup(x => x.GetProviderForChunk(It.IsAny<int>())).Returns(providerMock.Object);

        // Act
        var result = await _chunkService.ChunkAndStoreAsync(filePath);

        // Assert
        result.Should().NotBeEmpty();
        result.Select(x => x.ChunkId).Distinct().Count().Should().Be(result.Count); // Chunks must have unique Id.
        result.All(c => c.StorageProviderName == "FakeStorage").Should().BeTrue();

        File.Delete(filePath);
    }
}

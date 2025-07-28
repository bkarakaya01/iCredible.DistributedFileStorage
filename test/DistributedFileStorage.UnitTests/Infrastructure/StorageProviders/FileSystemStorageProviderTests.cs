using DistributedFileStorage.Infrastructure.StorageProviders;

namespace DistributedFileStorage.UnitTests.Infrastructure.StorageProviders;

public class FileSystemStorageProviderTests
{
    [Fact]
    public async Task SaveAndReadChunk_ShouldWorkCorrectly()
    {
        // Arrange
        var basePath = Path.Combine(Path.GetTempPath(), $"chunks_{Guid.NewGuid()}");
        var provider = new FileSystemStorageProvider(basePath);

        var chunkId = "test-chunk";
        var data = "test-data"u8.ToArray();

        // Act
        await provider.SaveChunkAsync(chunkId, data);
        var result = await provider.ReadChunkAsync(chunkId);

        // Assert
        Assert.Equal(data, result);

        // Cleanup
        var filePath = Path.Combine(basePath, $"{chunkId}.bin");
        if (File.Exists(filePath)) File.Delete(filePath);
        if (Directory.Exists(basePath)) Directory.Delete(basePath, true);
    }

    [Fact]
    public async Task ReadChunk_ShouldThrow_WhenFileNotExists()
    {
        var provider = new FileSystemStorageProvider(Path.GetTempPath());
        await Assert.ThrowsAsync<FileNotFoundException>(() =>
            provider.ReadChunkAsync("nonexistent"));
    }
}

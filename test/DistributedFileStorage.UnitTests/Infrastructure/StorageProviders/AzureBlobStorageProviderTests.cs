using DistributedFileStorage.Infrastructure.StorageProviders;

namespace DistributedFileStorage.UnitTests.Infrastructure.StorageProviders;

public class AzureBlobStorageProviderTests
{
    private const string ConnectionString = "UseDevelopmentStorage=true"; // Azurite
    private readonly string _containerName = $"test-container-{Guid.NewGuid()}";

    [Fact]
    public async Task SaveAndReadChunk_ShouldWorkCorrectly()
    {
        // Arrange
        var provider = new AzureBlobStorageProvider(ConnectionString, _containerName);
        var chunkId = "test-chunk";
        var data = "hello azure"u8.ToArray();

        // Act
        await provider.SaveChunkAsync(chunkId, data);
        var result = await provider.ReadChunkAsync(chunkId);

        // Assert
        Assert.Equal(data, result);
    }
}

using Azure.Storage.Blobs;
using DistributedFileStorage.Infrastructure.StorageProviders;
using System.Diagnostics;
using System.Net.Sockets;
using Xunit;

namespace DistributedFileStorage.UnitTests.Infrastructure.StorageProviders;

public class AzureBlobStorageProviderTests : IAsyncLifetime
{
    private const string ConnectionString = "UseDevelopmentStorage=true"; // Azurite
    private readonly string _containerName = $"test-container-{Guid.NewGuid()}";
    private AzureBlobStorageProvider _provider = null!;

    [Fact]
    public async Task SaveAndReadChunk_ShouldWorkCorrectly()
    {
        // Arrange
        var chunkId = "test-chunk";
        var data = "hello azure"u8.ToArray();

        // Act
        await _provider.SaveChunkAsync(chunkId, data);
        var result = await _provider.ReadChunkAsync(chunkId);

        // Assert
        Assert.Equal(data, result);
    }

    // Setup
    public async Task InitializeAsync()
    {
        await WaitForAzuriteAsync();

        _provider = new AzureBlobStorageProvider(ConnectionString, _containerName);

        var blobServiceClient = new BlobServiceClient(ConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.CreateIfNotExistsAsync();
    }

    // Cleanup
    public async Task DisposeAsync()
    {
        var blobServiceClient = new BlobServiceClient(ConnectionString);
        var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
        await containerClient.DeleteIfExistsAsync();
    }

    private static async Task WaitForAzuriteAsync(int timeoutMilliseconds = 5000)
    {
        using var client = new TcpClient();
        var sw = Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < timeoutMilliseconds)
        {
            try
            {
                await client.ConnectAsync("127.0.0.1", 10000);
                return;
            }
            catch
            {
                await Task.Delay(250);
            }
        }

        throw new Exception("Azurite endpoint is not available on 127.0.0.1:10000. Please install azurite module then in your terminal run => azurite --silent --location .azurite --debug .azurite/debug.log --skipApiVersionCheck");
    }
}

using Azure.Storage.Blobs;
using DistributedFileStorage.Domain.Interfaces.Storage;

namespace DistributedFileStorage.Infrastructure.StorageProviders;

public class AzureBlobStorageProvider : IStorageProvider
{
    private readonly BlobContainerClient _containerClient;
    public string Name => "AzureBlob";

    public AzureBlobStorageProvider(string connectionString, string containerName)
    {
        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists();
    }

    public async Task SaveChunkAsync(string chunkId, byte[] data)
    {
        var blobClient = _containerClient.GetBlobClient(chunkId);
        using var stream = new MemoryStream(data);
        await blobClient.UploadAsync(stream, overwrite: true);
    }

    public async Task<byte[]> ReadChunkAsync(string chunkId)
    {
        var blobClient = _containerClient.GetBlobClient(chunkId);
        using var stream = new MemoryStream();
        await blobClient.DownloadToAsync(stream);
        return stream.ToArray();
    }
}

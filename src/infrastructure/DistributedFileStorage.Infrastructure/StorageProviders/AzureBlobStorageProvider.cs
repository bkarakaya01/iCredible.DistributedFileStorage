using Azure.Storage.Blobs;
using DistributedFileStorage.Domain.Interfaces.Storage;

namespace DistributedFileStorage.Infrastructure.StorageProviders;

/// <summary>
/// Represents a storage provider that saves and retrieves chunks using Azure Blob Storage.
/// </summary>
public class AzureBlobStorageProvider : IStorageProvider
{
    private readonly BlobContainerClient _containerClient;

    /// <inheritdoc />
    public string Name => "AzureBlob";

    /// <summary>
    /// Initializes a new instance of the <see cref="AzureBlobStorageProvider"/> class.
    /// Ensures that the specified blob container exists or creates it if it does not.
    /// </summary>
    /// <param name="connectionString">The Azure Blob Storage connection string.</param>
    /// <param name="containerName">The name of the blob container to use for storing chunks.</param>
    public AzureBlobStorageProvider(string connectionString, string containerName)
    {
        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists();
    }

    /// <inheritdoc />
    public async Task SaveChunkAsync(string chunkId, byte[] data)
    {
        var blobClient = _containerClient.GetBlobClient(chunkId);
        using var stream = new MemoryStream(data);
        await blobClient.UploadAsync(stream, overwrite: true);
    }

    /// <inheritdoc />
    public async Task<byte[]> ReadChunkAsync(string chunkId)
    {
        var blobClient = _containerClient.GetBlobClient(chunkId);
        using var stream = new MemoryStream();
        await blobClient.DownloadToAsync(stream);
        return stream.ToArray();
    }
}

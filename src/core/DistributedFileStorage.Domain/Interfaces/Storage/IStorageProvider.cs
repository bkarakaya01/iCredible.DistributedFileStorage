namespace DistributedFileStorage.Domain.Interfaces.Storage;

/// <summary>
/// Defines a contract for a distributed storage provider capable of saving and reading file chunks.
/// </summary>
public interface IStorageProvider
{
    /// <summary>
    /// Gets the unique name of the storage provider.
    /// </summary>
    /// <remarks>
    /// This name is used to identify the provider in metadata and selection logic (e.g., "FileSystem", "AzureBlob").
    /// </remarks>
    string Name { get; }

    /// <summary>
    /// Asynchronously saves a binary chunk identified by a unique chunk ID.
    /// </summary>
    /// <param name="chunkId">The unique identifier of the chunk.</param>
    /// <param name="data">The binary content of the chunk.</param>
    /// <returns>A task that represents the asynchronous save operation.</returns>
    Task SaveChunkAsync(string chunkId, byte[] data);

    /// <summary>
    /// Asynchronously reads the binary content of a chunk by its unique identifier.
    /// </summary>
    /// <param name="chunkId">The unique identifier of the chunk to retrieve.</param>
    /// <returns>A task that returns the binary content of the chunk.</returns>
    Task<byte[]> ReadChunkAsync(string chunkId);
}

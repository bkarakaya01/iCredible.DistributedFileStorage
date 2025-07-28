using DistributedFileStorage.Domain.Interfaces.Storage;

namespace DistributedFileStorage.Domain.Interfaces;

/// <summary>
/// Defines a factory interface responsible for resolving the appropriate <see cref="IStorageProvider"/>
/// implementation based on chunk order or provider name.
/// </summary>
public interface IStorageProviderFactory
{
    /// <summary>
    /// Returns a storage provider instance responsible for storing or retrieving the chunk
    /// at the specified order in the file sequence.
    /// </summary>
    /// <param name="chunkOrder">The sequential order of the chunk in the file.</param>
    /// <returns>An <see cref="IStorageProvider"/> implementation suitable for the given chunk order.</returns>
    IStorageProvider GetProviderForChunk(int chunkOrder);

    /// <summary>
    /// Returns a storage provider instance corresponding to the given provider name.
    /// </summary>
    /// <param name="name">The name of the storage provider (e.g., "AzureBlob", "Redis").</param>
    /// <returns>An <see cref="IStorageProvider"/> implementation matching the specified name.</returns>
    IStorageProvider GetProviderByName(string name);
}

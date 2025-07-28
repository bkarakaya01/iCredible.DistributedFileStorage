using DistributedFileStorage.Domain.Interfaces.Storage;

namespace DistributedFileStorage.Infrastructure.StorageProviders;

/// <summary>
/// Represents a local file system-based storage provider for saving and retrieving file chunks.
/// </summary>
public class FileSystemStorageProvider : IStorageProvider
{
    private readonly string _basePath;

    /// <inheritdoc />
    public string Name => "FileSystem";

    /// <summary>
    /// Initializes a new instance of the <see cref="FileSystemStorageProvider"/> class.
    /// Ensures the base directory exists for storing file chunks.
    /// </summary>
    /// <param name="basePath">The base directory path where chunks will be stored as files.</param>
    public FileSystemStorageProvider(string basePath)
    {
        _basePath = basePath;
        Directory.CreateDirectory(_basePath);
    }

    /// <inheritdoc />
    public async Task SaveChunkAsync(string chunkId, byte[] data)
    {
        var filePath = Path.Combine(_basePath, $"{chunkId}.bin");
        await File.WriteAllBytesAsync(filePath, data);
    }

    /// <inheritdoc />
    public async Task<byte[]> ReadChunkAsync(string chunkId)
    {
        var filePath = Path.Combine(_basePath, $"{chunkId}.bin");

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Chunk {chunkId} not found on disk.");

        return await File.ReadAllBytesAsync(filePath);
    }
}

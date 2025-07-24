using DistributedFileStorage.Domain.Interfaces.Storage;

namespace DistributedFileStorage.Infrastructure.StorageProviders;

public class FileSystemStorageProvider : IStorageProvider
{
    private readonly string _basePath;
    public string Name => "FileSystem";

    public FileSystemStorageProvider(string basePath)
    {
        _basePath = basePath;
        Directory.CreateDirectory(_basePath);
    }

    public async Task SaveChunkAsync(string chunkId, byte[] data)
    {
        var filePath = Path.Combine(_basePath, $"{chunkId}.bin");
        await File.WriteAllBytesAsync(filePath, data);
    }

    public async Task<byte[]> ReadChunkAsync(string chunkId)
    {
        var filePath = Path.Combine(_basePath, $"{chunkId}.bin");

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Chunk {chunkId} not found on disk.");

        return await File.ReadAllBytesAsync(filePath);
    }
}

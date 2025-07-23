namespace DistributedFileStorage.Domain.Interfaces.Storage;

public interface IStorageProvider
{
    string Name { get; }
    Task SaveChunkAsync(string chunkId, byte[] data);
    Task<byte[]> ReadChunkAsync(string chunkId);
}

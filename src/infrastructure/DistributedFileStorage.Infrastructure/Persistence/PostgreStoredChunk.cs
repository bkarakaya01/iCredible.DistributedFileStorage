namespace DistributedFileStorage.Infrastructure.Persistence;

internal class PostgreStoredChunk
{
    public string ChunkId { get; set; } = null!;
    public byte[] Data { get; set; } = null!;
}

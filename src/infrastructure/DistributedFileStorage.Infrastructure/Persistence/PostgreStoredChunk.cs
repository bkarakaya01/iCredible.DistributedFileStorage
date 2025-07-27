namespace DistributedFileStorage.Infrastructure.Persistence;

public class PostgreStoredChunk
{
    public string ChunkId { get; set; } = null!;
    public byte[] Data { get; set; } = null!;
}

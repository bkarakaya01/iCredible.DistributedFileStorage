namespace DistributedFileStorage.Domain.Entities;

public class FileMetadata
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = null!;
    public long FileSize { get; set; }
    public string OriginalChecksum { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<ChunkMetadata> Chunks { get; set; } = [];
}
namespace DistributedFileStorage.Domain.Entities;

public class FileMetadata
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = null!;
    public long Size { get; set; }
    public string OriginalChecksum { get; set; } = null!;
    public List<ChunkMetadata> Chunks { get; set; } = [];
}

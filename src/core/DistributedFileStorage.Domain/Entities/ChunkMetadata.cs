namespace DistributedFileStorage.Domain.Entities;

public class ChunkMetadata
{
    public string ChunkId { get; set; } = null!;
    public int Order { get; set; }
    public long Size { get; set; }
    public string StorageProviderName { get; set; } = null!;

    public Guid FileMetadataId { get; set; }
    public FileMetadata File { get; set; } = null!;
}

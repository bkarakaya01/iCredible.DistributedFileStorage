namespace DistributedFileStorage.Domain.Entities;

/// <summary>
/// Represents metadata information for a complete file, including its name,
/// size, checksum, creation timestamp, and associated data chunks.
/// </summary>
public class FileMetadata
{
    /// <summary>
    /// Gets or sets the unique identifier for the file metadata.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the original name of the file.
    /// </summary>
    public string FileName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the total size of the file in bytes.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Gets or sets the checksum (SHA256) of the original file used for integrity validation.
    /// </summary>
    public string OriginalChecksum { get; set; } = null!;

    /// <summary>
    /// Gets or sets the UTC timestamp of when the file metadata was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the list of chunk metadata entities associated with this file.
    /// Each chunk represents a part of the original file.
    /// </summary>
    public List<ChunkMetadata> Chunks { get; set; } = [];
}

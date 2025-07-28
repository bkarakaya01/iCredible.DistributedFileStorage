using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DistributedFileStorage.Domain.Entities;

/// <summary>
/// Represents metadata information for a specific chunk of a file,
/// including its storage details, order within the original file, and size.
/// </summary>
public class ChunkMetadata
{
    /// <summary>
    /// Gets or sets the unique identifier of the chunk.
    /// This is typically a hash or UUID that distinguishes the chunk.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string ChunkId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the sequential order of the chunk in the file.
    /// Used to reconstruct the original file correctly.
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// Gets or sets the size of the chunk in bytes.
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Gets or sets the name of the storage provider where this chunk is stored.
    /// For example: "FileSystem", "AzureBlob", etc.
    /// </summary>
    public string StorageProviderName { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ID of the associated file metadata to which this chunk belongs.
    /// </summary>
    public Guid FileMetadataId { get; set; }

    /// <summary>
    /// Navigation property for the file that this chunk is part of.
    /// </summary>
    public FileMetadata File { get; set; } = null!;
}

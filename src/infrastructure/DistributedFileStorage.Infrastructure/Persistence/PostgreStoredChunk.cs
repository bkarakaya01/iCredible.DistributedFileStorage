namespace DistributedFileStorage.Infrastructure.Persistence;

/// <summary>
/// Represents a file chunk that is physically stored in a PostgreSQL database.
/// </summary>
public class PostgreStoredChunk
{
    /// <summary>
    /// Gets or sets the unique identifier of the chunk.
    /// This ID is used to reference and retrieve the chunk from storage.
    /// </summary>
    public string ChunkId { get; set; } = null!;

    /// <summary>
    /// Gets or sets the binary data of the chunk.
    /// </summary>
    public byte[] Data { get; set; } = null!;
}
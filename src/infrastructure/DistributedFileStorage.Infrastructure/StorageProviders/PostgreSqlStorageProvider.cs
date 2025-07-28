using DistributedFileStorage.Domain.Interfaces.Storage;
using DistributedFileStorage.Infrastructure.Persistence;

namespace DistributedFileStorage.Infrastructure.StorageProviders;

/// <summary>
/// Represents a storage provider that stores file chunks in a PostgreSQL database.
/// </summary>
public class PostgreSqlStorageProvider : IStorageProvider
{
    private readonly PostgreChunkDbContext _dbContext;

    /// <inheritdoc />
    public string Name => "PostgreSQL";

    /// <summary>
    /// Initializes a new instance of the <see cref="PostgreSqlStorageProvider"/> class.
    /// </summary>
    /// <param name="dbContext">The database context used to interact with the PostgreSQL storage.</param>
    public PostgreSqlStorageProvider(PostgreChunkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task SaveChunkAsync(string chunkId, byte[] data)
    {
        var entity = new PostgreStoredChunk
        {
            ChunkId = chunkId,
            Data = data
        };

        _dbContext.Chunks.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<byte[]> ReadChunkAsync(string chunkId)
    {
        var entity = await _dbContext.Chunks.FindAsync(chunkId);
        if (entity == null)
            throw new Exception($"Chunk {chunkId} not found in PostgreSQL");

        return entity.Data;
    }
}

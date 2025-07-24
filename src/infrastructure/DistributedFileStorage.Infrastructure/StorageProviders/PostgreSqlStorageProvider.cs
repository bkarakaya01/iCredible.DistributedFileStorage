using DistributedFileStorage.Domain.Interfaces.Storage;
using DistributedFileStorage.Infrastructure.Persistence;

namespace DistributedFileStorage.Infrastructure.StorageProviders;

public class PostgreSqlStorageProvider : IStorageProvider
{
    private readonly PostgreChunkDbContext _dbContext;
    public string Name => "PostgreSQL";

    public PostgreSqlStorageProvider(PostgreChunkDbContext dbContext)
    {
        _dbContext = dbContext;
    }

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

    public async Task<byte[]> ReadChunkAsync(string chunkId)
    {
        var entity = await _dbContext.Chunks.FindAsync(chunkId);
        if (entity == null)
            throw new Exception($"Chunk {chunkId} not found in PostgreSQL");

        return entity.Data;
    }
}

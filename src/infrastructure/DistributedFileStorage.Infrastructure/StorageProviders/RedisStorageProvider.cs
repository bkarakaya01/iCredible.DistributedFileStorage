using DistributedFileStorage.Domain.Interfaces.Storage;
using StackExchange.Redis;

namespace DistributedFileStorage.Infrastructure.StorageProviders;

public class RedisStorageProvider : IStorageProvider
{
    private readonly IDatabase _db;
    public string Name => "Redis";

    public RedisStorageProvider(IConnectionMultiplexer connectionMultiplexer)
    {
        _db = connectionMultiplexer.GetDatabase();
    }

    public Task SaveChunkAsync(string chunkId, byte[] data)
        => _db.StringSetAsync(chunkId, data);

    public async Task<byte[]> ReadChunkAsync(string chunkId)
    {
        var value = await _db.StringGetAsync(chunkId);
        if (!value.HasValue)
            throw new Exception($"Chunk {chunkId} not found in Redis");

        return value!;
    }
}

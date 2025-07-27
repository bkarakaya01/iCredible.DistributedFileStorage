using DistributedFileStorage.Domain.Entities;
using DistributedFileStorage.Domain.Interfaces;
using DistributedFileStorage.Domain.Interfaces.Storage;
using DistributedFileStorage.Domain.Interfaces.Strategies;
using DistributedFileStorage.Infrastructure.Persistence;
using System.Security.Cryptography;

namespace DistributedFileStorage.Application.Services;

public class ChunkService
{
    private readonly IStorageProviderFactory _providerFactory;
    private readonly IChunkingStrategy _chunkingStrategy;

    private readonly MetadataDbContext _dbContext;

    public ChunkService(
        IStorageProviderFactory providerFactory,
        IChunkingStrategy chunkingStrategy,
        MetadataDbContext dbContext)
    {
        _providerFactory = providerFactory;
        _chunkingStrategy = chunkingStrategy;
        _dbContext = dbContext;
    }

    public async Task<List<ChunkMetadata>> ChunkAndStoreAsync(string filePath)
    {
        long fileSize = new FileInfo(filePath).Length;
        int chunkSize = _chunkingStrategy.GetChunkSize(fileSize);

        List<ChunkMetadata> metadataList = [];

        using FileStream stream = File.OpenRead(filePath);
        byte[] buffer = new byte[chunkSize];
        int bytesRead;
        int order = 0;

        while ((bytesRead = await stream.ReadAsync(buffer.AsMemory(0, chunkSize))) > 0)
        {
            var chunk = new byte[bytesRead];
            Array.Copy(buffer, chunk, bytesRead);

            string chunkId = CalculateChunkId(chunk);
            IStorageProvider provider = _providerFactory.GetProviderForChunk(order);
            await provider.SaveChunkAsync(chunkId, chunk);

            metadataList.Add(new ChunkMetadata
            {
                ChunkId = chunkId,
                Order = order++,
                Size = chunk.Length,
                StorageProviderName = provider.Name
            });
        }

        var fileRecord = new FileMetadata
        {
            FileName = Path.GetFileName(filePath),
            FileSize = fileSize,
            OriginalChecksum = CalculateFileChecksum(filePath),
            Chunks = metadataList
        };

        _dbContext.Files.Add(fileRecord);
        await _dbContext.SaveChangesAsync();

        return metadataList;
    }

    private static string CalculateChunkId(byte[] data)
    {
        var hash = SHA256.HashData(data);
        return Convert.ToHexString(hash);
    }

    private static string CalculateFileChecksum(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = sha256.ComputeHash(stream);
        return Convert.ToHexString(hash);
    }
}

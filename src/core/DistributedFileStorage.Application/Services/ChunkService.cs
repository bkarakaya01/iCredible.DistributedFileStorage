using DistributedFileStorage.Application.Interfaces;
using DistributedFileStorage.Domain.Entities;
using DistributedFileStorage.Domain.Interfaces.Storage;
using DistributedFileStorage.Domain.Interfaces.Strategies;
using System.Security.Cryptography;

namespace DistributedFileStorage.Application.Services;

public class ChunkService
{
    private readonly IStorageProviderFactory _providerFactory;
    private readonly IChunkingStrategy _chunkingStrategy;

    public ChunkService(IStorageProviderFactory providerFactory, IChunkingStrategy chunkingStrategy)
    {
        _providerFactory = providerFactory;
        _chunkingStrategy = chunkingStrategy;
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

        return metadataList;
    }

    private static string CalculateChunkId(byte[] data)
    {
        var hash = SHA256.HashData(data);
        return Convert.ToHexString(hash);
    }
}

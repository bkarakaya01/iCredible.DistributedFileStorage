using DistributedFileStorage.Application.Utils;
using DistributedFileStorage.Domain.Entities;
using DistributedFileStorage.Domain.Interfaces;
using DistributedFileStorage.Domain.Interfaces.Storage;
using DistributedFileStorage.Domain.Interfaces.Strategies;
using DistributedFileStorage.Infrastructure.Persistence;

namespace DistributedFileStorage.Application.Services;

/// <summary>
/// Service responsible for splitting a file into chunks and storing each chunk
/// in a corresponding distributed storage provider, while also persisting metadata.
/// </summary>
public class ChunkService
{
    private readonly IStorageProviderFactory _providerFactory;
    private readonly IChunkingStrategy _chunkingStrategy;
    private readonly MetadataDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChunkService"/> class.
    /// </summary>
    /// <param name="providerFactory">Factory to determine which storage provider to use for a chunk.</param>
    /// <param name="chunkingStrategy">Strategy for calculating optimal chunk size based on file size.</param>
    /// <param name="dbContext">Database context for storing file and chunk metadata.</param>
    public ChunkService(
        IStorageProviderFactory providerFactory,
        IChunkingStrategy chunkingStrategy,
        MetadataDbContext dbContext)
    {
        _providerFactory = providerFactory;
        _chunkingStrategy = chunkingStrategy;
        _dbContext = dbContext;
    }

    /// <summary>
    /// Splits a file into chunks, stores them using distributed storage providers,
    /// and records metadata about the file and its chunks in the database.
    /// </summary>
    /// <param name="filePath">The full path to the file that will be chunked and stored.</param>
    /// <returns>A list of metadata objects describing each stored chunk.</returns>
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

            string chunkId = ChecksumHelper.CalculateChunkId(chunk);
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
            OriginalChecksum = ChecksumHelper.CalculateFileChecksum(filePath),
            Chunks = metadataList
        };

        _dbContext.Files.Add(fileRecord);
        await _dbContext.SaveChangesAsync();

        return metadataList;
    }
}

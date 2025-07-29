using DistributedFileStorage.Application.Utils;
using DistributedFileStorage.Domain.Entities;
using DistributedFileStorage.Domain.Interfaces;
using DistributedFileStorage.Domain.Interfaces.Storage;
using DistributedFileStorage.Domain.Interfaces.Strategies;
using DistributedFileStorage.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

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
    private readonly ILogger<ChunkService> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChunkService"/> class.
    /// </summary>
    /// <param name="providerFactory">Factory to determine which storage provider to use for a chunk.</param>
    /// <param name="chunkingStrategy">Strategy for calculating optimal chunk size based on file size.</param>
    /// <param name="dbContext">Database context for storing file and chunk metadata.</param>
    public ChunkService(
        IStorageProviderFactory providerFactory,
        IChunkingStrategy chunkingStrategy,
        MetadataDbContext dbContext,
        ILogger<ChunkService> logger)
    {
        _providerFactory = providerFactory;
        _chunkingStrategy = chunkingStrategy;
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>
    /// Splits a file into chunks, stores them using distributed storage providers,
    /// and records metadata about the file and its chunks in the database.
    /// </summary>
    /// <param name="filePath">The full path to the file that will be chunked and stored.</param>
    /// <returns>A list of metadata objects describing each stored chunk.</returns>
    public async Task<List<ChunkMetadata>> ChunkAndStoreAsync(string filePath)
    {
        _logger.LogInformation("Starting chunking process for file: {FilePath}", filePath);

        if (!File.Exists(filePath))
        {
            _logger.LogError("File not found at path: {FilePath}", filePath);
            throw new FileNotFoundException("The specified file does not exist.", filePath);
        }

        long fileSize = new FileInfo(filePath).Length;
        int chunkSize = _chunkingStrategy.GetChunkSize(fileSize);
        _logger.LogInformation("File size: {FileSize} bytes. Calculated chunk size: {ChunkSize} bytes.", fileSize, chunkSize);

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

            _logger.LogInformation("Saving chunk #{Order} (ID: {ChunkId}, Size: {Size} bytes) to provider: {Provider}",
                order, chunkId, chunk.Length, provider.Name);

            await provider.SaveChunkAsync(chunkId, chunk);

            metadataList.Add(new ChunkMetadata
            {
                ChunkId = chunkId,
                Order = order,
                Size = chunk.Length,
                StorageProviderName = provider.Name
            });

            _logger.LogInformation("Chunk #{Order} stored successfully into {Provider}.", order, provider.Name);
            order++;
        }

        var fileRecord = new FileMetadata
        {
            FileName = Path.GetFileName(filePath),
            FileSize = fileSize,
            OriginalChecksum = ChecksumHelper.CalculateFileChecksum(filePath),
            Chunks = metadataList
        };

        _logger.LogInformation("Persisting file metadata to database for file: {FileName} with {ChunkCount} chunks.",
            fileRecord.FileName, metadataList.Count);

        _dbContext.Files.Add(fileRecord);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("File metadata saved successfully. Chunking process completed.");

        return metadataList;
    }
}

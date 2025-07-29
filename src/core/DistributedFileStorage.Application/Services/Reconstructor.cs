using DistributedFileStorage.Application.Utils;
using DistributedFileStorage.Domain.Interfaces;
using DistributedFileStorage.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace DistributedFileStorage.Application.Services;

/// <summary>
/// Service responsible for reconstructing a previously chunked and distributed file.
/// It retrieves the chunk metadata and reassembles the original file in correct order.
/// </summary>
public class Reconstructor
{
    private readonly IStorageProviderFactory _providerFactory;
    private readonly MetadataDbContext _metadataDbContext;
    private readonly ILogger<Reconstructor> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Reconstructor"/> class.
    /// </summary>
    /// <param name="providerFactory">Factory to resolve the correct storage provider for each chunk.</param>
    /// <param name="metadataDbContext">Database context to retrieve file and chunk metadata.</param>
    /// <param name="logger">Logger instance for structured logging.</param>
    public Reconstructor(
        IStorageProviderFactory providerFactory, 
        MetadataDbContext metadataDbContext, 
        ILogger<Reconstructor> logger)
    {
        _providerFactory = providerFactory;
        _metadataDbContext = metadataDbContext;
        _logger = logger;
    }

    /// <summary>
    /// Reconstructs the original file from its stored chunks and writes it to the specified output path.
    /// Verifies integrity by comparing the computed checksum with the original one.
    /// </summary>
    /// <param name="fileId">The unique identifier of the file to be reconstructed.</param>
    /// <param name="outputPath">The path where the reconstructed file will be saved.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the file is not found or the checksum verification fails.</exception>
    public async Task ReconstructFileAsync(Guid fileId, string outputPath)
    {
        _logger.LogInformation("Starting file reconstruction for File ID: {FileId}", fileId);

        var file = await _metadataDbContext.Files.FindAsync(fileId);
        if (file == null)
        {
            _logger.LogError("File with ID {FileId} not found in metadata database.", fileId);
            throw new InvalidOperationException($"File with ID {fileId} not found.");
        }

        var orderedChunks = file.Chunks.OrderBy(c => c.Order).ToList();
        _logger.LogInformation("Found {ChunkCount} chunks. Beginning reconstruction...", orderedChunks.Count);

        await using var output = File.Create(outputPath);

        foreach (var chunk in orderedChunks)
        {
            _logger.LogDebug("Reading chunk #{Order} (Chunk ID: {ChunkId}) from {Provider}.", chunk.Order, chunk.ChunkId, chunk.StorageProviderName);
            var provider = _providerFactory.GetProviderByName(chunk.StorageProviderName);
            var data = await provider.ReadChunkAsync(chunk.ChunkId);
            await output.WriteAsync(data);
            _logger.LogInformation("Chunk #{Order} successfully written.", chunk.Order);
        }

        output.Close();
        _logger.LogInformation("File written to disk at: {OutputPath}", outputPath);

        var computedChecksum = ChecksumHelper.CalculateFileChecksum(outputPath);
        if (!string.Equals(computedChecksum, file.OriginalChecksum, StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogCritical("Checksum mismatch detected! Expected: {Expected}, Computed: {Computed}", file.OriginalChecksum, computedChecksum);
            throw new InvalidOperationException("Checksum mismatch! File integrity cannot be verified.");
        }

        _logger.LogInformation("File successfully reconstructed and verified. Path: {OutputPath}", outputPath);
    }
}

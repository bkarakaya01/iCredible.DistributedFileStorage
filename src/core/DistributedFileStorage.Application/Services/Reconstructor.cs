using DistributedFileStorage.Application.Utils;
using DistributedFileStorage.Domain.Interfaces;
using DistributedFileStorage.Infrastructure.Persistence;

namespace DistributedFileStorage.Application.Services
{
    /// <summary>
    /// Service responsible for reconstructing a previously chunked and distributed file.
    /// It retrieves the chunk metadata and reassembles the original file in correct order.
    /// </summary>
    public class Reconstructor
    {
        private readonly IStorageProviderFactory _providerFactory;
        private readonly MetadataDbContext _metadataDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="Reconstructor"/> class.
        /// </summary>
        /// <param name="providerFactory">Factory to resolve the correct storage provider for each chunk.</param>
        /// <param name="metadataDbContext">Database context to retrieve file and chunk metadata.</param>
        public Reconstructor(IStorageProviderFactory providerFactory, MetadataDbContext metadataDbContext)
        {
            _providerFactory = providerFactory;
            _metadataDbContext = metadataDbContext;
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
            var file = await _metadataDbContext.Files.FindAsync(fileId);
            if (file == null)
                throw new InvalidOperationException($"File with ID {fileId} not found.");

            var orderedChunks = file.Chunks.OrderBy(c => c.Order).ToList();

            await using var output = File.Create(outputPath);

            foreach (var chunk in orderedChunks)
            {
                var provider = _providerFactory.GetProviderByName(chunk.StorageProviderName);
                var data = await provider.ReadChunkAsync(chunk.ChunkId);
                await output.WriteAsync(data);
            }

            output.Close();

            var computedChecksum = ChecksumHelper.CalculateFileChecksum(outputPath);
            if (!string.Equals(computedChecksum, file.OriginalChecksum, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Checksum mismatch! File integrity cannot be verified.");

            Console.WriteLine($"==> File successfully reconstructed at: {outputPath}");
        }
    }
}

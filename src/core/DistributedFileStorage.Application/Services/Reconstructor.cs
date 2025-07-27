using DistributedFileStorage.Application.Utils;
using DistributedFileStorage.Domain.Interfaces;
using DistributedFileStorage.Infrastructure.Persistence;

namespace DistributedFileStorage.Application.Services
{
    public class Reconstructor
    {
        private readonly IStorageProviderFactory _providerFactory;
        private readonly MetadataDbContext _metadataDbContext;

        public Reconstructor(IStorageProviderFactory providerFactory, MetadataDbContext metadataDbContext)
        {
            _providerFactory = providerFactory;
            _metadataDbContext = metadataDbContext;
        }

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

            Console.WriteLine($"✅ File successfully reconstructed at: {outputPath}");
        }
    }
}

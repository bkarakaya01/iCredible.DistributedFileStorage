using DistributedFileStorage.Domain.Entities;
using DistributedFileStorage.Domain.Interfaces;

namespace DistributedFileStorage.Application.Services
{
    public class Reconstructor
    {
        private readonly IStorageProviderFactory _providerFactory;

        public Reconstructor(IStorageProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
        }

        public async Task RebuildFileAsync(List<ChunkMetadata> chunks, string outputPath)
        {
            var orderedChunks = chunks.OrderBy(x => x.Order);

            using var outputStream = File.Create(outputPath);
            foreach (var chunk in orderedChunks)
            {
                var provider = _providerFactory.GetProviderByName(chunk.StorageProviderName);
                var data = await provider.ReadChunkAsync(chunk.ChunkId);
                await outputStream.WriteAsync(data, 0, data.Length);
            }
        }
    }
}

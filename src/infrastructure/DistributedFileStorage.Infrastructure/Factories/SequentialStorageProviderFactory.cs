using DistributedFileStorage.Domain.Interfaces;
using DistributedFileStorage.Domain.Interfaces.Storage;

namespace DistributedFileStorage.Infrastructure.Factories;

public class SequentialStorageProviderFactory : IStorageProviderFactory
{
    private readonly List<IStorageProvider> _providers;

    public SequentialStorageProviderFactory(IEnumerable<IStorageProvider> providers)
    {
        _providers = [.. providers];

        if (_providers.Count == 0)
            throw new InvalidOperationException("At least one IStorageProvider must be registered.");
    }

    public IStorageProvider GetProviderByName(string name)
    {
        return null;
    }

    public IStorageProvider GetProviderForChunk(int chunkOrder)
    {
        return null;
    }
}

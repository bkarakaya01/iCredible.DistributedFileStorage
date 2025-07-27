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

    public IStorageProvider GetProviderForChunk(int chunkOrder)
    {
        int index = chunkOrder % _providers.Count;
        return _providers[index];
    }

    public IStorageProvider GetProviderByName(string name)
    {
        var provider = _providers.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return provider ?? throw new InvalidOperationException($"No storage provider found with name: {name}");
    }
}

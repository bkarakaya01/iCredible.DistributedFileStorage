using DistributedFileStorage.Domain.Interfaces;
using DistributedFileStorage.Domain.Interfaces.Storage;

namespace DistributedFileStorage.Infrastructure.Factories;

/// <summary>
/// A sequential storage provider factory that distributes file chunks across registered providers in round-robin order.
/// </summary>
public class SequentialStorageProviderFactory : IStorageProviderFactory
{
    private readonly List<IStorageProvider> _providers;

    /// <summary>
    /// Initializes the factory with a collection of available storage providers.
    /// </summary>
    /// <param name="providers">The list of available <see cref="IStorageProvider"/> instances.</param>
    /// <exception cref="InvalidOperationException">Thrown if the provider list is empty.</exception>
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

using DistributedFileStorage.Domain.Interfaces.Storage;

namespace DistributedFileStorage.Application.Interfaces;

public interface IStorageProviderFactory
{
    IStorageProvider GetProviderForChunk(int chunkOrder);
    IStorageProvider GetProviderByName(string name);
}

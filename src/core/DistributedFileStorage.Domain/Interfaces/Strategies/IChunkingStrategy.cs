namespace DistributedFileStorage.Domain.Interfaces.Strategies;

public interface IChunkingStrategy
{
    int GetChunkSize(long fileSize);
}

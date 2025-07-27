using DistributedFileStorage.Domain.Interfaces.Strategies;

namespace DistributedFileStorage.Infrastructure.Strategies;

public class OptimalChunkingStrategy : IChunkingStrategy
{
    private readonly int _minChunkSize;
    private readonly int _maxChunkSize;
    private readonly int _targetChunkCount;

    public OptimalChunkingStrategy(int targetChunkCount = 0, int minChunkSize = 0, int maxChunkSize = 0)
    {
        _targetChunkCount = targetChunkCount;
        _minChunkSize = minChunkSize;
        _maxChunkSize = maxChunkSize;
    }

    public int GetChunkSize(long fileSize)
    {
        return 0;
    }
}

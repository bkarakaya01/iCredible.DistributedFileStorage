using DistributedFileStorage.Domain.Interfaces.Strategies;

namespace DistributedFileStorage.Infrastructure.Strategies;

public class OptimalChunkingStrategy : IChunkingStrategy
{
    private readonly int _minChunkSize;
    private readonly int _maxChunkSize;
    private readonly int _targetChunkCount;

    public OptimalChunkingStrategy(int targetChunkCount = 50, int minChunkSize = 1 * 1024 * 1024, int maxChunkSize = 50 * 1024 * 1024)
    {
        _targetChunkCount = targetChunkCount;
        _minChunkSize = minChunkSize;
        _maxChunkSize = maxChunkSize;
    }

    public int GetChunkSize(long fileSize)
    {
        int ideal = (int)Math.Ceiling((double)fileSize / _targetChunkCount);
        return Math.Clamp(ideal, _minChunkSize, _maxChunkSize);
    }
}

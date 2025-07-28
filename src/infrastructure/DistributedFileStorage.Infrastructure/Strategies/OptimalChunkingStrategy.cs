using DistributedFileStorage.Domain.Interfaces.Strategies;

namespace DistributedFileStorage.Infrastructure.Strategies;

/// <summary>
/// Provides an optimal strategy for determining the chunk size of a file based on its total size.
/// The strategy attempts to divide the file into a target number of chunks, bounded by configurable minimum and maximum sizes.
/// </summary>
public class OptimalChunkingStrategy : IChunkingStrategy
{
    private readonly int _minChunkSize;
    private readonly int _maxChunkSize;
    private readonly int _targetChunkCount;

    /// <summary>
    /// Initializes a new instance of the <see cref="OptimalChunkingStrategy"/> class.
    /// </summary>
    /// <param name="targetChunkCount">The desired number of chunks to split the file into. Default is 50.</param>
    /// <param name="minChunkSize">The minimum allowed chunk size in bytes. Default is 1 MB.</param>
    /// <param name="maxChunkSize">The maximum allowed chunk size in bytes. Default is 50 MB.</param>
    public OptimalChunkingStrategy(int targetChunkCount = 50, int minChunkSize = 1 * 1024 * 1024, int maxChunkSize = 50 * 1024 * 1024)
    {
        _targetChunkCount = targetChunkCount;
        _minChunkSize = minChunkSize;
        _maxChunkSize = maxChunkSize;
    }

    /// <summary>
    /// Calculates an appropriate chunk size for the given file size, attempting to meet the target chunk count
    /// while ensuring the size falls between the minimum and maximum limits.
    /// </summary>
    /// <param name="fileSize">The total size of the file in bytes.</param>
    /// <returns>The calculated chunk size in bytes.</returns>
    public int GetChunkSize(long fileSize)
    {
        int ideal = (int)Math.Ceiling((double)fileSize / _targetChunkCount);
        return Math.Clamp(ideal, _minChunkSize, _maxChunkSize);
    }
}

namespace DistributedFileStorage.Domain.Interfaces.Strategies;

/// <summary>
/// Defines a strategy for determining the optimal chunk size for a given file.
/// </summary>
public interface IChunkingStrategy
{
    /// <summary>
    /// Calculates the appropriate chunk size based on the total file size.
    /// </summary>
    /// <param name="fileSize">The size of the file in bytes.</param>
    /// <returns>The size of each chunk in bytes.</returns>
    int GetChunkSize(long fileSize);
}
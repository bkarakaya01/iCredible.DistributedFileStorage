using DistributedFileStorage.Infrastructure.Strategies;

namespace DistributedFileStorage.UnitTests.Infrastructure.Strategies;

public class OptimalChunkingStrategyTests
{
    [Fact]
    public void GetChunkSize_ShouldReturn_MinChunkSize_When_FileIsSmall()
    {
        // Arrange
        var strategy = new OptimalChunkingStrategy(targetChunkCount: 50, minChunkSize: 1024, maxChunkSize: 10_240);
        long smallFileSize = 500; // 500 bytes < minChunkSize

        // Act
        int chunkSize = strategy.GetChunkSize(smallFileSize);

        // Assert
        Assert.Equal(1024, chunkSize);
    }

    [Fact]
    public void GetChunkSize_ShouldReturn_MaxChunkSize_When_FileIsLarge()
    {
        // Arrange
        var strategy = new OptimalChunkingStrategy(targetChunkCount: 10, minChunkSize: 1024, maxChunkSize: 1024 * 1024); // 1MB
        long largeFileSize = 100 * 1024 * 1024; // 100MB

        // Act
        int chunkSize = strategy.GetChunkSize(largeFileSize);

        // Assert
        Assert.Equal(1024 * 1024, chunkSize); // Max chunk size
    }

    [Fact]
    public void GetChunkSize_ShouldReturn_IdealChunkSize_WithinBounds()
    {
        // Arrange
        var strategy = new OptimalChunkingStrategy(targetChunkCount: 4, minChunkSize: 1024, maxChunkSize: 50_000);
        long fileSize = 16000; // 4 chunks = 4000 bytes ideal

        // Act
        int chunkSize = strategy.GetChunkSize(fileSize);

        // Assert
        Assert.Equal(4000, chunkSize);
    }

    [Fact]
    public void GetChunkSize_ShouldClamp_ToMin_When_IdealIsSmaller()
    {
        // Arrange
        var strategy = new OptimalChunkingStrategy(targetChunkCount: 1000, minChunkSize: 2048, maxChunkSize: 4096);
        long fileSize = 1_000_000; // ideal = 1000 bytes < min

        // Act
        int chunkSize = strategy.GetChunkSize(fileSize);

        // Assert
        Assert.Equal(2048, chunkSize);
    }

    [Fact]
    public void GetChunkSize_ShouldClamp_ToMax_When_IdealIsGreater()
    {
        // Arrange
        var strategy = new OptimalChunkingStrategy(targetChunkCount: 1, minChunkSize: 1024, maxChunkSize: 4096);
        long fileSize = 10_000; // ideal = 10000 > max

        // Act
        int chunkSize = strategy.GetChunkSize(fileSize);

        // Assert
        Assert.Equal(4096, chunkSize);
    }
}

using DistributedFileStorage.Infrastructure.Persistence;
using DistributedFileStorage.Infrastructure.StorageProviders;
using Microsoft.EntityFrameworkCore;

namespace DistributedFileStorage.UnitTests.Infrastructure.StorageProviders;

public class PostgreSqlStorageProviderTests
{
    [Fact]
    public async Task SaveAndReadChunk_ShouldWorkCorrectly()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<PostgreChunkDbContext>()
            .UseInMemoryDatabase(databaseName: $"PostgreTest_{Guid.NewGuid()}")
            .Options;

        using var context = new PostgreChunkDbContext(options);
        var provider = new PostgreSqlStorageProvider(context);

        var chunkId = Guid.NewGuid().ToString();
        var data = "test-db-chunk"u8.ToArray();

        // Act
        await provider.SaveChunkAsync(chunkId, data);
        var result = await provider.ReadChunkAsync(chunkId);

        // Assert
        Assert.Equal(data, result);
    }

    [Fact]
    public async Task ReadChunk_ShouldThrow_WhenNotFound()
    {
        var options = new DbContextOptionsBuilder<PostgreChunkDbContext>()
            .UseInMemoryDatabase($"NotFoundTest_{Guid.NewGuid()}")
            .Options;

        using var context = new PostgreChunkDbContext(options);
        var provider = new PostgreSqlStorageProvider(context);

        await Assert.ThrowsAsync<Exception>(() =>
            provider.ReadChunkAsync("missing-id"));
    }
}

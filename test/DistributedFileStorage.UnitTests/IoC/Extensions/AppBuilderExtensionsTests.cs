using DistributedFileStorage.Infrastructure.Persistence;
using DistributedFileStorage.IoC.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace DistributedFileStorage.UnitTests.IoC.Extensions;

public class AppBuilderExtensionsTests
{
    [Fact]
    public async Task SetupDbMigrations_ShouldApplyMigrationsWithoutException()
    {
        // Arrange
        var services = new ServiceCollection();

        var metaOptions = new DbContextOptionsBuilder<MetadataDbContext>()
            .UseInMemoryDatabase("MigrationTest_Metadata")
            .Options;

        var chunkOptions = new DbContextOptionsBuilder<PostgreChunkDbContext>()
            .UseInMemoryDatabase("MigrationTest_Chunk")
            .Options;

        services.AddSingleton(new MetadataDbContext(metaOptions));
        services.AddSingleton(new PostgreChunkDbContext(chunkOptions));

        var hostMock = new Mock<IHost>();
        var provider = services.BuildServiceProvider();
        hostMock.Setup(h => h.Services).Returns(provider);

        // Act & Assert
        var ex = await Record.ExceptionAsync(() => hostMock.Object.SetupDbMigrations());
        Assert.Null(ex); // exception fırlamamalı
    }
}

using DistributedFileStorage.Domain.Interfaces.Storage;
using DistributedFileStorage.Infrastructure.Factories;
using Moq;

namespace DistributedFileStorage.UnitTests.Infrastructure.Factories;

public class SequentialStorageProviderFactoryTests
{
    [Fact]
    public void GetProviderForChunk_ShouldReturnCorrectProvider_BasedOnChunkOrder()
    {
        // Arrange
        var provider1 = new Mock<IStorageProvider>();
        provider1.Setup(p => p.Name).Returns("Provider1");

        var provider2 = new Mock<IStorageProvider>();
        provider2.Setup(p => p.Name).Returns("Provider2");

        var provider3 = new Mock<IStorageProvider>();
        provider3.Setup(p => p.Name).Returns("Provider3");

        var providers = new List<IStorageProvider> { provider1.Object, provider2.Object, provider3.Object };
        var factory = new SequentialStorageProviderFactory(providers);

        // Act
        var result1 = factory.GetProviderForChunk(0);
        var result2 = factory.GetProviderForChunk(1);
        var result3 = factory.GetProviderForChunk(2);
        var result4 = factory.GetProviderForChunk(3);
        var result5 = factory.GetProviderForChunk(4);

        // Assert
        Assert.Equal("Provider1", result1.Name);
        Assert.Equal("Provider2", result2.Name);
        Assert.Equal("Provider3", result3.Name);
        Assert.Equal("Provider1", result4.Name); // döngü başa sarar
        Assert.Equal("Provider2", result5.Name);
    }

    [Fact]
    public void GetProviderByName_ShouldReturnCorrectProvider_WhenNameMatches()
    {
        // Arrange
        var provider = new Mock<IStorageProvider>();
        provider.Setup(p => p.Name).Returns("MyProvider");

        var factory = new SequentialStorageProviderFactory([provider.Object]);

        // Act
        var result = factory.GetProviderByName("MyProvider");

        // Assert
        Assert.Equal("MyProvider", result.Name);
    }

    [Fact]
    public void GetProviderByName_ShouldThrow_WhenProviderNotFound()
    {
        // Arrange
        var provider = new Mock<IStorageProvider>();
        provider.Setup(p => p.Name).Returns("Available");

        var factory = new SequentialStorageProviderFactory([provider.Object]);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => factory.GetProviderByName("Missing"));
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenNoProvidersGiven()
    {
        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => new SequentialStorageProviderFactory([]));
    }
}

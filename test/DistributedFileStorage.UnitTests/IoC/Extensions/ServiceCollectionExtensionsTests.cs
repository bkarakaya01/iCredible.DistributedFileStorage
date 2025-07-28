using DistributedFileStorage.Application.Services;
using DistributedFileStorage.Domain.Interfaces;
using DistributedFileStorage.Domain.Interfaces.Storage;
using DistributedFileStorage.Domain.Interfaces.Strategies;
using DistributedFileStorage.Infrastructure.StorageProviders;
using DistributedFileStorage.IoC.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DistributedFileStorage.UnitTests.IoC.Extensions;

public class ServiceCollectionExtensionsTests
{
    private readonly IServiceProvider _provider;

    public ServiceCollectionExtensionsTests()
    {
        var services = new ServiceCollection();

        // In-memory config
        var inMemorySettings = new Dictionary<string, string?>
        {
            ["Storage:PostgreChunk:ConnectionString"] = "Host=localhost;Database=chunkdb;Username=user;Password=pass",
            ["Storage:PostgreMetadata:ConnectionString"] = "Host=localhost;Database=metadatadb;Username=user;Password=pass",
            ["Storage:FileSystem:ChunkDirectory"] = "TestChunks",
            ["Storage:Azure:ConnectionString"] = "UseDevelopmentStorage=true",
            ["Storage:Azure:Container"] = "testcontainer"
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        services.AddDistributedFileStorage(configuration);

        _provider = services.BuildServiceProvider();
    }

    [Fact]
    public void AddDistributedFileStorage_ShouldRegister_ChunkService()
    {
        var service = _provider.GetService<ChunkService>();
        Assert.NotNull(service);
    }

    [Fact]
    public void AddDistributedFileStorage_ShouldRegister_Reconstructor()
    {
        var service = _provider.GetService<Reconstructor>();
        Assert.NotNull(service);
    }

    [Fact]
    public void AddDistributedFileStorage_ShouldRegister_IStorageProviderFactory()
    {
        var service = _provider.GetService<IStorageProviderFactory>();
        Assert.NotNull(service);
    }

    [Fact]
    public void AddDistributedFileStorage_ShouldRegister_IChunkingStrategy()
    {
        var service = _provider.GetService<IChunkingStrategy>();
        Assert.NotNull(service);
    }

    [Fact]
    public void AddDistributedFileStorage_ShouldRegister_FileSystemStorageProvider()
    {
        var providers = _provider.GetServices<IStorageProvider>().ToList();
        var fsProvider = providers.FirstOrDefault(p => p.Name == "FileSystem");

        Assert.NotNull(fsProvider);
        Assert.IsType<FileSystemStorageProvider>(fsProvider);
    }

    [Fact]
    public void AddDistributedFileStorage_ShouldRegister_AzureBlobStorageProvider()
    {
        var providers = _provider.GetServices<IStorageProvider>().ToList();
        var azureProvider = providers.FirstOrDefault(p => p.Name == "AzureBlob");

        Assert.NotNull(azureProvider);
        Assert.IsType<AzureBlobStorageProvider>(azureProvider);
    }

    [Fact]
    public void AddDistributedFileStorage_ShouldRegister_PostgreSqlStorageProvider()
    {
        var providers = _provider.GetServices<IStorageProvider>().ToList();
        var pgProvider = providers.FirstOrDefault(p => p.Name == "PostgreSQL");

        Assert.NotNull(pgProvider);
        Assert.IsType<PostgreSqlStorageProvider>(pgProvider);
    }

}

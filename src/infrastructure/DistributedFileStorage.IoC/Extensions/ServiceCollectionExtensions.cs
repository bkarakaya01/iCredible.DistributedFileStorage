using DistributedFileStorage.Application.Services;
using DistributedFileStorage.Domain.Interfaces;
using DistributedFileStorage.Domain.Interfaces.Storage;
using DistributedFileStorage.Domain.Interfaces.Strategies;
using DistributedFileStorage.Infrastructure.Factories;
using DistributedFileStorage.Infrastructure.Persistence;
using DistributedFileStorage.Infrastructure.StorageProviders;
using DistributedFileStorage.Infrastructure.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace DistributedFileStorage.IoC.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistributedFileStorage(this IServiceCollection services, IConfiguration config)
    {
        // DbContexts
        services.AddDbContext<MetadataDbContext>(options =>
            options.UseNpgsql(config["Storage:PostgreSql:ConnectionString"]));

        services.AddDbContext<PostgreChunkDbContext>(options =>
            options.UseNpgsql(config["Storage:PostgreSql:ConnectionString"]));

        // Chunking strategy
        services.AddSingleton<IChunkingStrategy, OptimalChunkingStrategy>();

        // Storage Providers
        services.AddSingleton<IStorageProvider>(sp =>
        {
            var conn = config["Storage:Redis:ConnectionString"];
            return new RedisStorageProvider(conn);
        });

        services.AddSingleton<IStorageProvider>(sp =>
        {
            var path = "chunks"; // Local path
            return new FileSystemStorageProvider(path);
        });

        services.AddSingleton<IStorageProvider>(sp =>
        {
            var conn = config["Storage:AzureBlob:ConnectionString"];
            var container = config["Storage:AzureBlob:ContainerName"];
            return new AzureBlobStorageProvider(conn, container);
        });

        services.AddScoped<IStorageProvider, PostgreSqlStorageProvider>();

        // Factory (RoundRobin)
        services.AddSingleton<IStorageProviderFactory>(sp =>
        {
            List<IStorageProvider> providers = [.. sp.GetServices<IStorageProvider>()];
            return new SequentialStorageProviderFactory(providers);
        });

        // Application Services
        services.AddScoped<ChunkService>();
        services.AddScoped<Reconstructor>();

        return services;
    }
}

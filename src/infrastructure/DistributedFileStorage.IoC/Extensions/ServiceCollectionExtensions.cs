using DistributedFileStorage.Application.Services;
using DistributedFileStorage.Domain.Interfaces;
using DistributedFileStorage.Domain.Interfaces.Storage;
using DistributedFileStorage.Domain.Interfaces.Strategies;
using DistributedFileStorage.Infrastructure.Factories;
using DistributedFileStorage.Infrastructure.Persistence;
using DistributedFileStorage.Infrastructure.StorageProviders;
using DistributedFileStorage.Infrastructure.Strategies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace DistributedFileStorage.IoC.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDistributedFileStorage(this IServiceCollection services, IConfiguration config)
    {
        // 1. DbContexts (PostgreChunk ve PostgreMetadata)
        services.AddDbContext<PostgreChunkDbContext>(options =>
            options.UseNpgsql(config["Storage:PostgreChunk:ConnectionString"]));

        services.AddDbContext<MetadataDbContext>(options =>
            options.UseNpgsql(config["Storage:PostgreMetadata:ConnectionString"]));

        // 2. Redis bağlantısı (ConnectionMultiplexer singleton)
        services.AddSingleton<IConnectionMultiplexer>(sp =>
            ConnectionMultiplexer.Connect(config["Storage:Redis:ConnectionString"]));

        // 3. Storage Providers

        // Redis
        services.AddSingleton<RedisStorageProvider>();
        services.AddSingleton<IStorageProvider>(sp => sp.GetRequiredService<RedisStorageProvider>());

        // FileSystem
        services.AddSingleton<IStorageProvider>(sp =>
        {
            var path = config["Storage:FileSystem:ChunkDirectory"] ?? "chunks";
            return new FileSystemStorageProvider(path);
        });

        // Azure Blob
        services.AddSingleton<IStorageProvider>(sp =>
        {
            var conn = config["Storage:Azure:ConnectionString"];
            var container = config["Storage:Azure:Container"];
            return new AzureBlobStorageProvider(conn, container);
        });

        // PostgreSQL
        services.AddScoped<IStorageProvider, PostgreSqlStorageProvider>();

        // 4. Chunking Strategy
        services.AddSingleton<IChunkingStrategy, OptimalChunkingStrategy>();

        // 5. Storage Provider Factory
        services.AddSingleton<IStorageProviderFactory>(sp =>
        {
            var allProviders = sp.GetServices<IStorageProvider>().ToList();
            return new SequentialStorageProviderFactory(allProviders);
        });

        // 6. Application Services
        services.AddScoped<ChunkService>();
        services.AddScoped<Reconstructor>();

        return services;
    }
}

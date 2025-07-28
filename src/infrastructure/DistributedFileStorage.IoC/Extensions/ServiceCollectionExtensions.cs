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

namespace DistributedFileStorage.IoC.Extensions;

/// <summary>
/// Extension methods for registering distributed file storage related services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures all services required for distributed file storage,
    /// including DbContexts, storage providers, chunking strategies, and core services.
    /// </summary>
    /// <param name="services">The service collection to register dependencies into.</param>
    /// <param name="config">The application configuration for retrieving connection strings and paths.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> instance.</returns>
    public static IServiceCollection AddDistributedFileStorage(this IServiceCollection services, IConfiguration config)
    {
        // 1. Database contexts for storing chunks and metadata
        services.AddDbContext<PostgreChunkDbContext>(options =>
            options.UseNpgsql(config["Storage:PostgreChunk:ConnectionString"]));

        services.AddDbContext<MetadataDbContext>(options =>
            options.UseNpgsql(config["Storage:PostgreMetadata:ConnectionString"]));

        // 2. Storage providers

        // File system storage provider
        services.AddSingleton<IStorageProvider>(sp =>
        {
            var path = config["Storage:FileSystem:ChunkDirectory"] ?? "chunks";
            return new FileSystemStorageProvider(path);
        });

        // Azure Blob storage provider
        services.AddSingleton<IStorageProvider>(sp =>
        {
            var conn = config["Storage:Azure:ConnectionString"];
            var container = config["Storage:Azure:Container"];
            return new AzureBlobStorageProvider(conn, container);
        });

        // PostgreSQL-based storage provider for chunks
        services.AddScoped<IStorageProvider, PostgreSqlStorageProvider>();

        // 3. Chunking strategy service
        services.AddSingleton<IChunkingStrategy, OptimalChunkingStrategy>();

        // 4. Factory for selecting the appropriate storage provider
        services.AddSingleton<IStorageProviderFactory>(sp =>
        {
            var allProviders = sp.GetServices<IStorageProvider>().ToList();
            return new SequentialStorageProviderFactory(allProviders);
        });

        // 5. Application-level services for chunking and reconstruction
        services.AddScoped<ChunkService>();
        services.AddScoped<Reconstructor>();

        return services;
    }
}

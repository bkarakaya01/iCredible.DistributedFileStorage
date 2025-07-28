using DistributedFileStorage.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DistributedFileStorage.IoC.Extensions;

/// <summary>
/// Extension methods for configuring and initializing application services.
/// </summary>
public static class AppBuilderExtensions
{
    /// <summary>
    /// Applies any pending EF Core migrations for both the metadata and chunk databases.
    /// This method ensures that the database schemas are up-to-date with the current models at runtime.
    /// </summary>
    /// <param name="host">The <see cref="IHost"/> instance used to resolve services.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <exception cref="Exception">Throws if migration fails for any context.</exception>
    public static async Task SetupDbMigrations(this IHost host)
    {
        try
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            await services.GetRequiredService<MetadataDbContext>().Database.MigrateAsync();
            await services.GetRequiredService<PostgreChunkDbContext>().Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error was encountered while trying to apply db migrations: {ex.Message}");
            throw;
        }
    }
}

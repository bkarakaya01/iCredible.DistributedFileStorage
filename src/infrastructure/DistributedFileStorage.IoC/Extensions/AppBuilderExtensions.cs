using DistributedFileStorage.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DistributedFileStorage.IoC.Extensions;

public static class AppBuilderExtensions
{
    public static async Task InitDbAsync(this IHost host)
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
            Console.WriteLine($"An error was encountered while trying to apply db migrations.", ex.Message);
			throw;
		}
    }
}

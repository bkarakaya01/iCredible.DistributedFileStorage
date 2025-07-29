using DistributedFileStorage.Application.Services;
using DistributedFileStorage.IoC.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .CreateLogger();

try
{
    Log.Information("Starting application...");

    var host = Host.CreateDefaultBuilder(args)
        .UseSerilog()
        .ConfigureAppConfiguration((context, config) =>
        {
            config
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        })
        .ConfigureServices((context, services) =>
        {
            services.AddDistributedFileStorage(context.Configuration);
        })
        .Build();

    await host.SetupDbMigrations();

    using var scope = host.Services.CreateScope();
    var provider = scope.ServiceProvider;

    // Dosya yolları
    var filePath = Path.Combine(AppContext.BaseDirectory, "Files", "sample.pdf");
    var outputPath = Path.Combine(AppContext.BaseDirectory, "Files", "reconstructed.pdf");

    try
    {
        Log.Information("-> Chunking...");
        var chunkService = provider.GetRequiredService<ChunkService>();
        var chunks = await chunkService.ChunkAndStoreAsync(filePath);

        Log.Information("-> Reconstructing...");
        var reconstructor = provider.GetRequiredService<Reconstructor>();
        await reconstructor.ReconstructFileAsync(chunks.First().FileMetadataId, outputPath);

        Log.Information("-> Operation completed successfully.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "-> An error occurred during chunk/reconstruct operation.");
    }

    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

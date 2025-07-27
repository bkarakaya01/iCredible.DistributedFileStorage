using DistributedFileStorage.Application.Services;
using DistributedFileStorage.IoC.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
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

using var scope = host.Services.CreateScope();
var provider = scope.ServiceProvider;

// Dosya yolları
string filePath = Path.Combine("Files", "sample.pdf");
string outputPath = Path.Combine("Files", "reconstructed.pdf");

try
{
    Console.WriteLine("-> Chunking...");
    var chunkService = provider.GetRequiredService<ChunkService>();
    var chunks = await chunkService.ChunkAndStoreAsync(filePath);

    Console.WriteLine("-> Reconstructing...");
    var reconstructor = provider.GetRequiredService<Reconstructor>();
    await reconstructor.ReconstructFileAsync(chunks.First().FileMetadataId, outputPath);

    Console.WriteLine("-> İşlem tamamlandı.");
}
catch (Exception ex)
{
    Console.WriteLine($"-> Hata oluştu: {ex.Message}");
}

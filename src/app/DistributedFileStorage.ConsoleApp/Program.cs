using DistributedFileStorage.Application.Services;
using DistributedFileStorage.IoC.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json");

var configuration = builder.Build();

var services = new ServiceCollection();
services.AddDistributedFileStorage(configuration);

var provider = services.BuildServiceProvider();

var chunkService = provider.GetRequiredService<ChunkService>();
var reconstructor = provider.GetRequiredService<Reconstructor>();

string filePath = Path.Combine("Files", "sample.pdf");
string outputPath = Path.Combine("Files", "reconstructed.pdf");

Console.WriteLine("-> Chunking...");
var chunks = await chunkService.ChunkAndStoreAsync(filePath);

Console.WriteLine("-> Reconstructing...");
await reconstructor.ReconstructFileAsync(chunks.First().FileMetadataId, outputPath);

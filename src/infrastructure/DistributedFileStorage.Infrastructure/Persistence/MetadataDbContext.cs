using DistributedFileStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DistributedFileStorage.Infrastructure.Persistence;

public class MetadataDbContext : DbContext
{
    public MetadataDbContext(DbContextOptions<MetadataDbContext> options) : base(options) { }

    public DbSet<FileMetadata> Files { get; set; }
    public DbSet<ChunkMetadata> Chunks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FileMetadata>()
            .HasMany(f => f.Chunks)
            .WithOne(c => c.File)
            .HasForeignKey(c => c.FileMetadataId);
    }
}

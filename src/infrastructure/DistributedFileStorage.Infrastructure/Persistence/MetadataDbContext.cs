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
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ChunkMetadata>(entity =>
        {
            entity.HasKey(e => e.ChunkId);
            entity.Property(e => e.ChunkId).IsRequired();
            entity.Property(e => e.StorageProviderName).IsRequired();
            entity.HasOne(e => e.File)
                  .WithMany(f => f.Chunks)
                  .HasForeignKey(e => e.FileMetadataId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<FileMetadata>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired();
            entity.Property(e => e.OriginalChecksum).IsRequired();
        });
    }
}

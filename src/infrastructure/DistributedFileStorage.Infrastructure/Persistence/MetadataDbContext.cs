using DistributedFileStorage.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DistributedFileStorage.Infrastructure.Persistence;

/// <summary>
/// Represents the Entity Framework Core database context for storing file and chunk metadata.
/// </summary>
public class MetadataDbContext : DbContext
{
    public MetadataDbContext(DbContextOptions<MetadataDbContext> options) : base(options) { }

    /// <summary>
    /// Gets or sets the file metadata entities.
    /// </summary>
    public DbSet<FileMetadata> Files { get; set; }

    /// <summary>
    /// Gets or sets the chunk metadata entities.
    /// </summary>
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

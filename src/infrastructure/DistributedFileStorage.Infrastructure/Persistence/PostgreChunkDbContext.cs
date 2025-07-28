using Microsoft.EntityFrameworkCore;
using DistributedFileStorage.Infrastructure.StorageProviders;

namespace DistributedFileStorage.Infrastructure.Persistence;

/// <summary>
/// Represents the EF Core database context for storing file chunks in a PostgreSQL database for <see cref="PostgreSqlStorageProvider"/>.
/// </summary>
public class PostgreChunkDbContext : DbContext
{
    public DbSet<PostgreStoredChunk> Chunks { get; set; }

    public PostgreChunkDbContext(DbContextOptions<PostgreChunkDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PostgreStoredChunk>().HasKey(x => x.ChunkId);
    }
}

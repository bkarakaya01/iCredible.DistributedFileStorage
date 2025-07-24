using Microsoft.EntityFrameworkCore;

namespace DistributedFileStorage.Infrastructure.Persistence
{
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
}

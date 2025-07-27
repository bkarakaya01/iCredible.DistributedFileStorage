using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DistributedFileStorage.Infrastructure.Migrations.ChunkDb
{
    /// <inheritdoc />
    public partial class InitChunkDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chunks",
                columns: table => new
                {
                    ChunkId = table.Column<string>(type: "text", nullable: false),
                    Data = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chunks", x => x.ChunkId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chunks");
        }
    }
}

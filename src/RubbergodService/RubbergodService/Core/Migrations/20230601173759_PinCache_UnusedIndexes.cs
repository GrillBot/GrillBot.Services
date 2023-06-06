using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RubbergodService.Core.Migrations
{
    /// <inheritdoc />
    public partial class PinCache_UnusedIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PinCache_CreatedAtUtc",
                table: "PinCache");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_PinCache_CreatedAtUtc",
                table: "PinCache",
                column: "CreatedAtUtc");
        }
    }
}

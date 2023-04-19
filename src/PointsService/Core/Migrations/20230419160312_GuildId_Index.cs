using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PointsService.Core.Migrations
{
    /// <inheritdoc />
    public partial class GuildId_Index : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_GuildId",
                table: "Transactions",
                column: "GuildId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_GuildId",
                table: "Transactions");
        }
    }
}

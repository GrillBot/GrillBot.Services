using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PointsService.Core.Migrations
{
    /// <inheritdoc />
    public partial class GuildId_Index2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_GuildId_MessageId_MergedCount",
                table: "Transactions",
                columns: new[] { "GuildId", "MessageId", "MergedCount" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_GuildId_MessageId_MergedCount",
                table: "Transactions");
        }
    }
}

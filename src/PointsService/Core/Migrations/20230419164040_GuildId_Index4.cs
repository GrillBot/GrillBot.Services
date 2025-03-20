using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PointsService.Core.Migrations
{
    /// <inheritdoc />
    public partial class GuildId_Index4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_GuildId_MessageId_MergedCount",
                table: "Transactions");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_GuildId_MessageId",
                table: "Transactions",
                columns: ["GuildId", "MessageId"]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_GuildId_MessageId",
                table: "Transactions");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_GuildId_MessageId_MergedCount",
                table: "Transactions",
                columns: ["GuildId", "MessageId", "MergedCount"]);
        }
    }
}

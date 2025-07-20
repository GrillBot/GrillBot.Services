using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnverifyService.Core.Migrations
{
    /// <inheritdoc />
    public partial class GuildOwnerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "OwnerUserId",
                table: "Guilds",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerUserId",
                table: "Guilds");
        }
    }
}

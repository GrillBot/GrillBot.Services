using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RubbergodService.Core.Migrations
{
    /// <inheritdoc />
    public partial class PinCache_RemovedUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PinCache",
                table: "PinCache");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PinCache");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PinCache",
                table: "PinCache",
                columns: new[] { "GuildId", "ChannelId", "Filename" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PinCache",
                table: "PinCache");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "PinCache",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PinCache",
                table: "PinCache",
                columns: new[] { "GuildId", "ChannelId", "UserId", "Filename" });
        }
    }
}

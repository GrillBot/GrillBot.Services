using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RubbergodService.Core.Migrations
{
    /// <inheritdoc />
    public partial class PinCache_StoreAsFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PinCache",
                table: "PinCache");

            migrationBuilder.DropIndex(
                name: "IX_PinCache_IsValid",
                table: "PinCache");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "PinCache");

            migrationBuilder.AddColumn<string>(
                name: "Filename",
                table: "PinCache",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PinCache",
                table: "PinCache",
                columns: new[] { "GuildId", "ChannelId", "UserId", "Filename" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PinCache",
                table: "PinCache");

            migrationBuilder.DropColumn(
                name: "Filename",
                table: "PinCache");

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "PinCache",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PinCache",
                table: "PinCache",
                columns: new[] { "GuildId", "ChannelId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_PinCache_IsValid",
                table: "PinCache",
                column: "IsValid");
        }
    }
}

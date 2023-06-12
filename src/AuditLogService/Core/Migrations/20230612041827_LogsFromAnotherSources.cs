using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations
{
    /// <inheritdoc />
    public partial class LogsFromAnotherSources : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "GuildInfoItems");

            migrationBuilder.DropColumn(
                name: "VoiceRegionId",
                table: "GuildInfoItems");

            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "LogMessages",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SourceAppName",
                table: "LogMessages",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE \"LogMessages\" SET \"Source\"='Legacy', \"SourceAppName\"='GrillBot'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Source",
                table: "LogMessages");

            migrationBuilder.DropColumn(
                name: "SourceAppName",
                table: "LogMessages");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "GuildInfoItems",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VoiceRegionId",
                table: "GuildInfoItems",
                type: "text",
                nullable: true);
        }
    }
}

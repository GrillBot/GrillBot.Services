using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmoteService.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmoteDefinitions",
                columns: table => new
                {
                    EmoteId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    EmoteName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EmoteIsAnimated = table.Column<bool>(type: "boolean", nullable: false),
                    GuildId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmoteDefinitions", x => new { x.EmoteId, x.EmoteName, x.EmoteIsAnimated, x.GuildId });
                });

            migrationBuilder.CreateTable(
                name: "EmoteUserStatItems",
                columns: table => new
                {
                    EmoteId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    EmoteName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    EmoteIsAnimated = table.Column<bool>(type: "boolean", nullable: false),
                    GuildId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UserId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    UseCount = table.Column<long>(type: "bigint", nullable: false),
                    FirstOccurence = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastOccurence = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmoteUserStatItems", x => new { x.EmoteId, x.EmoteName, x.EmoteIsAnimated, x.GuildId, x.UserId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmoteDefinitions");

            migrationBuilder.DropTable(
                name: "EmoteUserStatItems");
        }
    }
}

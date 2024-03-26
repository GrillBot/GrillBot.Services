using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RubbergodService.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Karma",
                columns: table => new
                {
                    MemberId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    KarmaValue = table.Column<int>(type: "integer", nullable: false),
                    Positive = table.Column<int>(type: "integer", nullable: false),
                    Negative = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Karma", x => x.MemberId);
                });

            migrationBuilder.CreateTable(
                name: "MemberCache",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Discriminator = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    AvatarUrl = table.Column<string>(type: "text", nullable: false),
                    InsertedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberCache", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MembeCacheItem_InsertedAt",
                table: "MemberCache",
                column: "InsertedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Karma");

            migrationBuilder.DropTable(
                name: "MemberCache");
        }
    }
}

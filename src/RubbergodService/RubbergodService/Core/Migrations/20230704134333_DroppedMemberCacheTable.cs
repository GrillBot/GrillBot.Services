using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RubbergodService.Core.Migrations
{
    /// <inheritdoc />
    public partial class DroppedMemberCacheTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemberCache");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MemberCache",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    AvatarUrl = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberCache", x => x.UserId);
                });
        }
    }
}

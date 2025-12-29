using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagementService.Core.Migrations
{
    /// <inheritdoc />
    public partial class FixMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Nicknames_GuildId_UserId",
                table: "Nicknames",
                columns: new[] { "GuildId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Nicknames_GuildUsers_GuildId_UserId",
                table: "Nicknames",
                columns: new[] { "GuildId", "UserId" },
                principalTable: "GuildUsers",
                principalColumns: new[] { "GuildId", "UserId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Nicknames_GuildUsers_GuildId_UserId",
                table: "Nicknames");

            migrationBuilder.DropIndex(
                name: "IX_Nicknames_GuildId_UserId",
                table: "Nicknames");
        }
    }
}

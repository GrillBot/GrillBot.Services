using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InviteService.Core.Migrations
{
    /// <inheritdoc />
    public partial class ForeignKeyFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InviteUses_Invites_GuildId_Code",
                table: "InviteUses");

            migrationBuilder.DropIndex(
                name: "IX_InviteUses_GuildId_Code",
                table: "InviteUses");

            migrationBuilder.CreateIndex(
                name: "IX_InviteUses_Code_GuildId",
                table: "InviteUses",
                columns: new[] { "Code", "GuildId" });

            migrationBuilder.AddForeignKey(
                name: "FK_InviteUses_Invites_Code_GuildId",
                table: "InviteUses",
                columns: new[] { "Code", "GuildId" },
                principalTable: "Invites",
                principalColumns: new[] { "Code", "GuildId" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InviteUses_Invites_Code_GuildId",
                table: "InviteUses");

            migrationBuilder.DropIndex(
                name: "IX_InviteUses_Code_GuildId",
                table: "InviteUses");

            migrationBuilder.CreateIndex(
                name: "IX_InviteUses_GuildId_Code",
                table: "InviteUses",
                columns: new[] { "GuildId", "Code" });

            migrationBuilder.AddForeignKey(
                name: "FK_InviteUses_Invites_GuildId_Code",
                table: "InviteUses",
                columns: new[] { "GuildId", "Code" },
                principalTable: "Invites",
                principalColumns: new[] { "Code", "GuildId" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}

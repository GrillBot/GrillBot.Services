using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MessageService.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AutoReplyDefinitions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Template = table.Column<string>(type: "text", nullable: false),
                    Reply = table.Column<string>(type: "text", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsCaseSensitive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AutoReplyDefinitions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GuildChannels",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "numeric", nullable: false),
                    ChannelId = table.Column<decimal>(type: "numeric", nullable: false),
                    IsAutoReplyDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsPointsDisabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildChannels", x => new { x.GuildId, x.ChannelId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AutoReplyDefinitions");

            migrationBuilder.DropTable(
                name: "GuildChannels");
        }
    }
}

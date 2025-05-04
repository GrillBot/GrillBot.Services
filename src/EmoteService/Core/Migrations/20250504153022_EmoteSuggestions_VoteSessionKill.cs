using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmoteService.Core.Migrations
{
    /// <inheritdoc />
    public partial class EmoteSuggestions_VoteSessionKill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "KilledAtUtc",
                schema: "suggestions",
                table: "EmoteVoteSessions",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KilledAtUtc",
                schema: "suggestions",
                table: "EmoteVoteSessions");
        }
    }
}

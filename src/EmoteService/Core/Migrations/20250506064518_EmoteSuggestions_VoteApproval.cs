using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmoteService.Core.Migrations
{
    /// <inheritdoc />
    public partial class EmoteSuggestions_VoteApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ApprovalByUserId",
                schema: "suggestions",
                table: "EmoteSuggestions",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalSetAtUtc",
                schema: "suggestions",
                table: "EmoteSuggestions",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalByUserId",
                schema: "suggestions",
                table: "EmoteSuggestions");

            migrationBuilder.DropColumn(
                name: "ApprovalSetAtUtc",
                schema: "suggestions",
                table: "EmoteSuggestions");
        }
    }
}

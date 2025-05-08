using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmoteService.Core.Migrations
{
    /// <inheritdoc />
    public partial class EmoteSuggestions_VoteClose : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                schema: "suggestions",
                table: "EmoteVoteSessions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsClosed",
                schema: "suggestions",
                table: "EmoteVoteSessions");
        }
    }
}

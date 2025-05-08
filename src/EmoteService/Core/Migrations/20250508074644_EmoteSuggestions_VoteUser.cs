using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmoteService.Core.Migrations
{
    /// <inheritdoc />
    public partial class EmoteSuggestions_VoteUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "UserId",
                schema: "suggestions",
                table: "EmoteUserVotes",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "suggestions",
                table: "EmoteUserVotes");
        }
    }
}

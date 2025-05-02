using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmoteService.Core.Migrations
{
    /// <inheritdoc />
    public partial class EmoteSuggestions_AdditionalData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAnimated",
                schema: "suggestions",
                table: "EmoteSuggestions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ReasonForAdd",
                schema: "suggestions",
                table: "EmoteSuggestions",
                type: "character varying(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAnimated",
                schema: "suggestions",
                table: "EmoteSuggestions");

            migrationBuilder.DropColumn(
                name: "ReasonForAdd",
                schema: "suggestions",
                table: "EmoteSuggestions");
        }
    }
}

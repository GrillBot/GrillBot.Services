using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PointsService.Core.Migrations
{
    /// <inheritdoc />
    public partial class UserContainerBotInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUser",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUser",
                table: "Users");
        }
    }
}

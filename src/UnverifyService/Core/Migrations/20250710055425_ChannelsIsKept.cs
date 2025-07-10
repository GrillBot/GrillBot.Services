using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnverifyService.Core.Migrations
{
    /// <inheritdoc />
    public partial class ChannelsIsKept : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsKept",
                schema: "logs",
                table: "UnverifyLogSetChannel",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsKept",
                schema: "logs",
                table: "UnverifyLogSetChannel");
        }
    }
}

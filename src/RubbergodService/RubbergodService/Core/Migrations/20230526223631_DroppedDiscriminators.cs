using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RubbergodService.Core.Migrations
{
    /// <inheritdoc />
    public partial class DroppedDiscriminators : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "MemberCache");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "MemberCache",
                type: "character varying(4)",
                maxLength: 4,
                nullable: false,
                defaultValue: "");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserMeasuresService.Core.Migrations
{
    /// <inheritdoc />
    public partial class UnverifyLogSetId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "LogSetId",
                table: "Unverifies",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogSetId",
                table: "Unverifies");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PointsService.Core.Migrations
{
    /// <inheritdoc />
    public partial class EntityRevision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PendingRecalculation",
                table: "Users");

            migrationBuilder.Sql("DELETE FROM public.\"DailyStats\"");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PendingRecalculation",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PointsService.Core.Migrations
{
    /// <inheritdoc />
    public partial class PointsUserStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ActivePoints",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "ExpiredPoints",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "MergedPoints",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.Sql("UPDATE public.\"Users\" SET \"PendingRecalculation\"=true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivePoints",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ExpiredPoints",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "MergedPoints",
                table: "Users");
        }
    }
}

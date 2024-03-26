using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations
{
    /// <inheritdoc />
    public partial class ApiRequestRequestdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "RequestDate",
                table: "ApiRequests",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.Sql("UPDATE \"ApiRequests\" SET \"RequestDate\" = \"EndAt\"::DATE");

            migrationBuilder.CreateIndex(
                name: "IX_ApiRequests_RequestDate",
                table: "ApiRequests",
                column: "RequestDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApiRequests_RequestDate",
                table: "ApiRequests");

            migrationBuilder.DropColumn(
                name: "RequestDate",
                table: "ApiRequests");
        }
    }
}

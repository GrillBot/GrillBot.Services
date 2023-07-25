using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations
{
    /// <inheritdoc />
    public partial class ApiRequestRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "ApiRequests",
                type: "text",
                nullable: true);

            migrationBuilder.Sql("UPDATE \"ApiRequests\" SET \"Role\" = 'Admin' WHERE \"Identification\" LIKE 'ApiV1(Private/%' AND \"ApiGroupName\" = 'V1'");
            migrationBuilder.Sql("UPDATE \"ApiRequests\" SET \"Role\" = 'User' WHERE \"Identification\" LIKE 'ApiV1(Public/%' AND \"ApiGroupName\" = 'V1'");

            migrationBuilder.CreateIndex(
                name: "IX_ApiRequests_Role",
                table: "ApiRequests",
                column: "Role",
                filter: "\"Role\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApiRequests_Role",
                table: "ApiRequests");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "ApiRequests");
        }
    }
}

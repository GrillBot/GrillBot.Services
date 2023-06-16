using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations
{
    /// <inheritdoc />
    public partial class DatabaseIndexe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_JobExecutions_EndAt",
                table: "JobExecutions",
                column: "EndAt");

            migrationBuilder.CreateIndex(
                name: "IX_ApiRequests_ApiGroupName",
                table: "ApiRequests",
                column: "ApiGroupName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobExecutions_EndAt",
                table: "JobExecutions");

            migrationBuilder.DropIndex(
                name: "IX_ApiRequests_ApiGroupName",
                table: "ApiRequests");
        }
    }
}

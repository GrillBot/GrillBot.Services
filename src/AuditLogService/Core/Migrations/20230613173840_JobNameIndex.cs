using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations
{
    /// <inheritdoc />
    public partial class JobNameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_JobExecutions_JobName",
                table: "JobExecutions",
                column: "JobName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobExecutions_JobName",
                table: "JobExecutions");
        }
    }
}

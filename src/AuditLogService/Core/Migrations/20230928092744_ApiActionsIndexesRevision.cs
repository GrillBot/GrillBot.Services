using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations
{
    /// <inheritdoc />
    public partial class ApiActionsIndexesRevision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_LogItem_CreatedAt",
                table: "LogItems",
                newName: "IX_LogItems_CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_LogItems_Type_IsDeleted",
                table: "LogItems",
                columns: new[] { "Type", "IsDeleted" });

            migrationBuilder.CreateIndex(
                name: "IX_ApiRequests_Duration",
                table: "ApiRequests",
                column: "Duration");

            migrationBuilder.CreateIndex(
                name: "IX_ApiRequests_Method_TemplatePath_IsSuccess",
                table: "ApiRequests",
                columns: new[] { "Method", "TemplatePath", "IsSuccess" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LogItems_Type_IsDeleted",
                table: "LogItems");

            migrationBuilder.DropIndex(
                name: "IX_ApiRequests_Duration",
                table: "ApiRequests");

            migrationBuilder.DropIndex(
                name: "IX_ApiRequests_Method_TemplatePath_IsSuccess",
                table: "ApiRequests");

            migrationBuilder.RenameIndex(
                name: "IX_LogItems_CreatedAt",
                table: "LogItems",
                newName: "IX_LogItem_CreatedAt");
        }
    }
}

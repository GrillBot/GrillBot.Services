using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations
{
    /// <inheritdoc />
    public partial class ApiRequestIndexApiUserStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "ApiRequests",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiRequests_LogItemId_Method_TemplatePath_ApiGroupName_Iden~",
                table: "ApiRequests",
                columns: new[] { "LogItemId", "Method", "TemplatePath", "ApiGroupName", "Identification" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApiRequests_LogItemId_Method_TemplatePath_ApiGroupName_Iden~",
                table: "ApiRequests");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "ApiRequests",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(32)",
                oldMaxLength: 32,
                oldNullable: true);
        }
    }
}

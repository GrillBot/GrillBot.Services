using AuditLogService.Core.Entity;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations
{
    /// <inheritdoc />
    public partial class DbOpts_20250922 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApiRequests_ApiGroupName",
                table: "ApiRequests");

            migrationBuilder.DropIndex(
                name: "IX_ApiRequests_Duration",
                table: "ApiRequests");

            migrationBuilder.DropIndex(
                name: "IX_ApiRequests_EndAt",
                table: "ApiRequests");

            migrationBuilder.DropIndex(
                name: "IX_ApiRequests_LogItemId_Method_TemplatePath_ApiGroupName_Iden~",
                table: "ApiRequests");

            migrationBuilder.DropIndex(
                name: "IX_ApiRequests_Method_TemplatePath",
                table: "ApiRequests");

            migrationBuilder.DropIndex(
                name: "IX_ApiRequests_Method_TemplatePath_IsSuccess",
                table: "ApiRequests");

            migrationBuilder.DropIndex(
                name: "IX_ApiRequests_RequestDate",
                table: "ApiRequests");

            migrationBuilder.AlterColumn<string>(
                name: "Tags",
                table: "ThreadInfoItems",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "Permissions",
                table: "RoleInfos",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(List<string>),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "Parameters",
                table: "InteractionCommands",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(List<InteractionCommandParameter>),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "Parameters",
                table: "ApiRequests",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(Dictionary<string, string>),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<string>(
                name: "Headers",
                table: "ApiRequests",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(Dictionary<string, string>),
                oldType: "jsonb");

            migrationBuilder.AlterColumn<int>(
                name: "Duration",
                table: "ApiRequests",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.CreateIndex(
                name: "IX_ApiRequests_ApiGroupName_EndAt",
                table: "ApiRequests",
                columns: new[] { "ApiGroupName", "EndAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ApiRequests_ApiGroupName_RequestDate",
                table: "ApiRequests",
                columns: new[] { "ApiGroupName", "RequestDate" });

            migrationBuilder.CreateIndex(
                name: "IX_ApiRequests_Method_TemplatePath_ApiGroupName_Identification",
                table: "ApiRequests",
                columns: new[] { "Method", "TemplatePath", "ApiGroupName", "Identification" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApiRequests_ApiGroupName_EndAt",
                table: "ApiRequests");

            migrationBuilder.DropIndex(
                name: "IX_ApiRequests_ApiGroupName_RequestDate",
                table: "ApiRequests");

            migrationBuilder.DropIndex(
                name: "IX_ApiRequests_Method_TemplatePath_ApiGroupName_Identification",
                table: "ApiRequests");

            migrationBuilder.AlterColumn<List<string>>(
                name: "Tags",
                table: "ThreadInfoItems",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<List<string>>(
                name: "Permissions",
                table: "RoleInfos",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<List<InteractionCommandParameter>>(
                name: "Parameters",
                table: "InteractionCommands",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<Dictionary<string, string>>(
                name: "Parameters",
                table: "ApiRequests",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<Dictionary<string, string>>(
                name: "Headers",
                table: "ApiRequests",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "Duration",
                table: "ApiRequests",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_ApiRequests_ApiGroupName",
                table: "ApiRequests",
                column: "ApiGroupName");

            migrationBuilder.CreateIndex(
                name: "IX_ApiRequests_Duration",
                table: "ApiRequests",
                column: "Duration");

            migrationBuilder.CreateIndex(
                name: "IX_ApiRequests_EndAt",
                table: "ApiRequests",
                column: "EndAt");

            migrationBuilder.CreateIndex(
                name: "IX_ApiRequests_LogItemId_Method_TemplatePath_ApiGroupName_Iden~",
                table: "ApiRequests",
                columns: new[] { "LogItemId", "Method", "TemplatePath", "ApiGroupName", "Identification" });

            migrationBuilder.CreateIndex(
                name: "IX_ApiRequests_Method_TemplatePath",
                table: "ApiRequests",
                columns: new[] { "Method", "TemplatePath" });

            migrationBuilder.CreateIndex(
                name: "IX_ApiRequests_Method_TemplatePath_IsSuccess",
                table: "ApiRequests",
                columns: new[] { "Method", "TemplatePath", "IsSuccess" });

            migrationBuilder.CreateIndex(
                name: "IX_ApiRequests_RequestDate",
                table: "ApiRequests",
                column: "RequestDate");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations
{
    /// <inheritdoc />
    public partial class LogItemFlags_AsBooleans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "LogItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPendingProcess",
                table: "LogItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE public.\"LogItems\" SET \"IsDeleted\"=true WHERE (\"Flags\" & 2) > 0");
            migrationBuilder.Sql("UPDATE public.\"LogItems\" SET \"IsPendingProcess\"=true WHERE (\"Flags\" & 1) > 0");

            migrationBuilder.DropColumn(
                name: "Flags",
                table: "LogItems");

            migrationBuilder.CreateIndex(
                name: "IX_LogItems_IsDeleted",
                table: "LogItems",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LogItems_IsPendingProcess",
                table: "LogItems",
                column: "IsPendingProcess");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LogItems_IsDeleted",
                table: "LogItems");

            migrationBuilder.DropIndex(
                name: "IX_LogItems_IsPendingProcess",
                table: "LogItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "LogItems");

            migrationBuilder.DropColumn(
                name: "IsPendingProcess",
                table: "LogItems");

            migrationBuilder.AddColumn<long>(
                name: "Flags",
                table: "LogItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }
    }
}

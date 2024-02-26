using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations
{
    /// <inheritdoc />
    public partial class RabbitMQIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LogItems_Type_IsDeleted",
                table: "LogItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "LogItems");

            migrationBuilder.DropColumn(
                name: "IsPendingProcess",
                table: "LogItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateIndex(
                name: "IX_LogItems_Type_IsDeleted",
                table: "LogItems",
                columns: new[] { "Type", "IsDeleted" });
        }
    }
}

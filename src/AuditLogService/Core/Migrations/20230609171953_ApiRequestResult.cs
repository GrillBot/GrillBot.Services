using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations
{
    /// <inheritdoc />
    public partial class ApiRequestResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "ApiRequests",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result",
                table: "ApiRequests");
        }
    }
}

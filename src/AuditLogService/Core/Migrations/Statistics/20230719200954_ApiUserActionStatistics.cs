using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations.Statistics
{
    /// <inheritdoc />
    public partial class ApiUserActionStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiUserActionStatistics",
                schema: "statistics",
                columns: table => new
                {
                    Action = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    UserId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    ApiGroup = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    IsPublic = table.Column<bool>(type: "boolean", nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiUserActionStatistics", x => new { x.Action, x.ApiGroup, x.IsPublic, x.UserId });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiUserActionStatistics",
                schema: "statistics");
        }
    }
}

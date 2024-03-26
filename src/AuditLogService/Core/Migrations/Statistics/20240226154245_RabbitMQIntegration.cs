using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations.Statistics
{
    /// <inheritdoc />
    public partial class RabbitMQIntegration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DateCountStatistics",
                schema: "statistics");

            migrationBuilder.DropTable(
                name: "DateStatistics",
                schema: "statistics");

            migrationBuilder.DropTable(
                name: "InteractionDateCountStatistics",
                schema: "statistics");

            migrationBuilder.DropTable(
                name: "TypeStatistics",
                schema: "statistics");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DateCountStatistics",
                schema: "statistics",
                columns: table => new
                {
                    ApiGroup = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DateCountStatistics", x => new { x.ApiGroup, x.Date });
                });

            migrationBuilder.CreateTable(
                name: "DateStatistics",
                schema: "statistics",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DateStatistics", x => x.Date);
                });

            migrationBuilder.CreateTable(
                name: "InteractionDateCountStatistics",
                schema: "statistics",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InteractionDateCountStatistics", x => x.Date);
                });

            migrationBuilder.CreateTable(
                name: "TypeStatistics",
                schema: "statistics",
                columns: table => new
                {
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeStatistics", x => x.Type);
                });
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations.Statistics
{
    /// <inheritdoc />
    public partial class DatabaseStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResultCountStatistic",
                schema: "statistics");

            migrationBuilder.CreateTable(
                name: "DatabaseStatistics",
                schema: "statistics",
                columns: table => new
                {
                    TableName = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    RecordsCount = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatabaseStatistics", x => x.TableName);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatabaseStatistics",
                schema: "statistics");

            migrationBuilder.CreateTable(
                name: "ResultCountStatistic",
                schema: "statistics",
                columns: table => new
                {
                    ApiGroup = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    Result = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultCountStatistic", x => new { x.ApiGroup, x.Result });
                });
        }
    }
}

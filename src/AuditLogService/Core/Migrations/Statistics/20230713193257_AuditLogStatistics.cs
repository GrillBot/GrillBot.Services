using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations.Statistics
{
    /// <inheritdoc />
    public partial class AuditLogStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "statistics");

            migrationBuilder.CreateTable(
                name: "DateCountStatistics",
                schema: "statistics",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    ApiGroup = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DateCountStatistics", x => new { x.ApiGroup, x.Date });
                });

            migrationBuilder.CreateTable(
                name: "RequestStats",
                schema: "statistics",
                columns: table => new
                {
                    Endpoint = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    LastRequest = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FailedCount = table.Column<long>(type: "bigint", nullable: false),
                    MaxDuration = table.Column<long>(type: "bigint", nullable: false),
                    MinDuration = table.Column<long>(type: "bigint", nullable: false),
                    SuccessCount = table.Column<long>(type: "bigint", nullable: false),
                    TotalDuration = table.Column<long>(type: "bigint", nullable: false),
                    LastRunDuration = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestStats", x => x.Endpoint);
                });

            migrationBuilder.CreateTable(
                name: "ResultCountStatistic",
                schema: "statistics",
                columns: table => new
                {
                    Result = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ApiGroup = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultCountStatistic", x => new { x.ApiGroup, x.Result });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DateCountStatistics",
                schema: "statistics");

            migrationBuilder.DropTable(
                name: "RequestStats",
                schema: "statistics");

            migrationBuilder.DropTable(
                name: "ResultCountStatistic",
                schema: "statistics");
        }
    }
}

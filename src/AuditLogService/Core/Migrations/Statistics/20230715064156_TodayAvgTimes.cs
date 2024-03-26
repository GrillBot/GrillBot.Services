using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations.Statistics
{
    /// <inheritdoc />
    public partial class TodayAvgTimes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyAvgTimes",
                schema: "statistics",
                columns: table => new
                {
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Interactions = table.Column<double>(type: "double precision", nullable: false),
                    Jobs = table.Column<double>(type: "double precision", nullable: false),
                    ExternalApi = table.Column<double>(type: "double precision", nullable: false),
                    InternalApi = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyAvgTimes", x => x.Date);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyAvgTimes",
                schema: "statistics");
        }
    }
}

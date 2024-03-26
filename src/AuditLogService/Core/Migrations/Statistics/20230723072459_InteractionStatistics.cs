using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations.Statistics
{
    /// <inheritdoc />
    public partial class InteractionStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InteractionStatistics",
                schema: "statistics",
                columns: table => new
                {
                    Action = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false),
                    LastRun = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FailedCount = table.Column<long>(type: "bigint", nullable: false),
                    MaxDuration = table.Column<long>(type: "bigint", nullable: false),
                    MinDuration = table.Column<long>(type: "bigint", nullable: false),
                    SuccessCount = table.Column<long>(type: "bigint", nullable: false),
                    TotalDuration = table.Column<long>(type: "bigint", nullable: false),
                    LastRunDuration = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InteractionStatistics", x => x.Action);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InteractionStatistics",
                schema: "statistics");
        }
    }
}

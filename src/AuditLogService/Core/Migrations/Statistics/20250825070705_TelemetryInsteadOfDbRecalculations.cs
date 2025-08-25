using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations.Statistics
{
    /// <inheritdoc />
    public partial class TelemetryInsteadOfDbRecalculations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatabaseStatistics",
                schema: "statistics");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}

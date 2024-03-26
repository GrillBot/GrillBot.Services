using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations.Statistics
{
    /// <inheritdoc />
    public partial class JobInfos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobInfos",
                schema: "statistics",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    StartCount = table.Column<int>(type: "integer", nullable: false),
                    LastRunDuration = table.Column<int>(type: "integer", nullable: false),
                    LastStartAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FailedCount = table.Column<int>(type: "integer", nullable: false),
                    TotalDuration = table.Column<int>(type: "integer", nullable: false),
                    MinTime = table.Column<int>(type: "integer", nullable: false),
                    MaxTime = table.Column<int>(type: "integer", nullable: false),
                    AvgTime = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobInfos", x => x.Name);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobInfos",
                schema: "statistics");
        }
    }
}

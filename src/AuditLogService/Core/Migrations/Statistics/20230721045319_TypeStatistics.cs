using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations.Statistics
{
    /// <inheritdoc />
    public partial class TypeStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TypeStatistics",
                schema: "statistics");
        }
    }
}

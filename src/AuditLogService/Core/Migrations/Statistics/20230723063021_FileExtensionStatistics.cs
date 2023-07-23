using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations.Statistics
{
    /// <inheritdoc />
    public partial class FileExtensionStatistics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileExtensionStatistics",
                schema: "statistics",
                columns: table => new
                {
                    Extension = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Count = table.Column<long>(type: "bigint", nullable: false),
                    Size = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileExtensionStatistics", x => x.Extension);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileExtensionStatistics",
                schema: "statistics");
        }
    }
}

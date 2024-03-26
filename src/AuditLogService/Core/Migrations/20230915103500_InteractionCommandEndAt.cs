using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations
{
    /// <inheritdoc />
    public partial class InteractionCommandEndAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndAt",
                table: "InteractionCommands",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.Sql("UPDATE public.\"InteractionCommands\" c1 SET \"EndAt\"=(SELECT \"CreatedAt\" FROM \"InteractionCommands\" c JOIN \"LogItems\" l ON l.\"Id\"=c.\"LogItemId\" AND c.\"LogItemId\"=c1.\"LogItemId\")");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndAt",
                table: "InteractionCommands");
        }
    }
}

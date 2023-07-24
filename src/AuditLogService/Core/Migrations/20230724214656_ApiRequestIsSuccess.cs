using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations
{
    /// <inheritdoc />
    public partial class ApiRequestIsSuccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSuccess",
                table: "ApiRequests",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("UPDATE \"ApiRequests\" SET \"IsSuccess\" = true WHERE \"Result\" IN ('200 (OK)', '302 (Redirect)', '204 (NoContent)')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSuccess",
                table: "ApiRequests");
        }
    }
}

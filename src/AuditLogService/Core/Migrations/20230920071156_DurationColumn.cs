using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations
{
    /// <inheritdoc />
    public partial class DurationColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Duration",
                table: "JobExecutions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Duration",
                table: "ApiRequests",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.Sql("UPDATE \"JobExecutions\" j1 SET \"Duration\"=(select round(date_part('epoch', \"EndAt\" - \"StartAt\") / 0.001)::int from \"JobExecutions\" j2 WHERE j1.\"LogItemId\"=j2.\"LogItemId\")");
            migrationBuilder.Sql("UPDATE \"ApiRequests\" a SET \"Duration\"=(select round(date_part('epoch', \"EndAt\" - \"StartAt\") / 0.001)::int from \"ApiRequests\" a1 WHERE a.\"LogItemId\"=a1.\"LogItemId\")");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "JobExecutions");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "ApiRequests");
        }
    }
}

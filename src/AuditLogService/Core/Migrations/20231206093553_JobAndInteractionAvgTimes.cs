using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations
{
    /// <inheritdoc />
    public partial class JobAndInteractionAvgTimes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "JobDate",
                table: "JobExecutions",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.Sql("UPDATE \"JobExecutions\" SET \"JobDate\"=\"EndAt\"::DATE");

            migrationBuilder.AddColumn<DateOnly>(
                name: "InteractionDate",
                table: "InteractionCommands",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.Sql("UPDATE \"InteractionCommands\" SET \"InteractionDate\"=\"EndAt\"::DATE");

            migrationBuilder.CreateIndex(
                name: "IX_JobExecutions_JobDate",
                table: "JobExecutions",
                column: "JobDate");

            migrationBuilder.CreateIndex(
                name: "IX_InteractionCommands_InteractionDate",
                table: "InteractionCommands",
                column: "InteractionDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_JobExecutions_JobDate",
                table: "JobExecutions");

            migrationBuilder.DropIndex(
                name: "IX_InteractionCommands_InteractionDate",
                table: "InteractionCommands");

            migrationBuilder.DropColumn(
                name: "JobDate",
                table: "JobExecutions");

            migrationBuilder.DropColumn(
                name: "InteractionDate",
                table: "InteractionCommands");
        }
    }
}

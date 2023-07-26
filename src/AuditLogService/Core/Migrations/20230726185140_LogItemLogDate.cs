using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations
{
    /// <inheritdoc />
    public partial class LogItemLogDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LogItems_IsDeleted",
                table: "LogItems");

            migrationBuilder.DropIndex(
                name: "IX_LogItems_IsPendingProcess",
                table: "LogItems");

            migrationBuilder.AddColumn<DateOnly>(
                name: "LogDate",
                table: "LogItems",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.Sql("UPDATE \"LogItems\" SET \"LogDate\" = \"CreatedAt\"::DATE");

            migrationBuilder.CreateIndex(
                name: "IX_LogItems_LogDate",
                table: "LogItems",
                column: "LogDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LogItems_LogDate",
                table: "LogItems");

            migrationBuilder.DropColumn(
                name: "LogDate",
                table: "LogItems");

            migrationBuilder.CreateIndex(
                name: "IX_LogItems_IsDeleted",
                table: "LogItems",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_LogItems_IsPendingProcess",
                table: "LogItems",
                column: "IsPendingProcess");
        }
    }
}

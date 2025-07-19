using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnverifyService.Core.Migrations
{
    /// <inheritdoc />
    public partial class UnverifyLogsParentChild : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentLogItemId",
                schema: "logs",
                table: "LogItems",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LogItems_ParentLogItemId",
                schema: "logs",
                table: "LogItems",
                column: "ParentLogItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_LogItems_LogItems_ParentLogItemId",
                schema: "logs",
                table: "LogItems",
                column: "ParentLogItemId",
                principalSchema: "logs",
                principalTable: "LogItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogItems_LogItems_ParentLogItemId",
                schema: "logs",
                table: "LogItems");

            migrationBuilder.DropIndex(
                name: "IX_LogItems_ParentLogItemId",
                schema: "logs",
                table: "LogItems");

            migrationBuilder.DropColumn(
                name: "ParentLogItemId",
                schema: "logs",
                table: "LogItems");
        }
    }
}

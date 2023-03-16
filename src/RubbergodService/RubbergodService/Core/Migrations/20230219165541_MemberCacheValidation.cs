using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RubbergodService.Core.Migrations
{
    /// <inheritdoc />
    public partial class MemberCacheValidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MembeCacheItem_InsertedAt",
                table: "MemberCache");

            migrationBuilder.DropColumn(
                name: "InsertedAt",
                table: "MemberCache");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "InsertedAt",
                table: "MemberCache",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_MembeCacheItem_InsertedAt",
                table: "MemberCache",
                column: "InsertedAt");
        }
    }
}

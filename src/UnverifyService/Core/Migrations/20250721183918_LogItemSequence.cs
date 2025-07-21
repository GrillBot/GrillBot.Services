using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace UnverifyService.Core.Migrations
{
    /// <inheritdoc />
    public partial class LogItemSequence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                 name: "LogNumber",
                 schema: "logs",
                 table: "LogItems",
                 type: "bigint",
                 nullable: false)
                 .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.Sql("ALTER SEQUENCE logs.\"LogItems_LogNumber_seq\" RESTART WITH 20000;");

            migrationBuilder.CreateIndex(
                name: "IX_LogItems_LogNumber",
                schema: "logs",
                table: "LogItems",
                column: "LogNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LogItems_LogNumber",
                schema: "logs",
                table: "LogItems");

            migrationBuilder.DropColumn(
                name: "LogNumber",
                schema: "logs",
                table: "LogItems");
        }
    }
}

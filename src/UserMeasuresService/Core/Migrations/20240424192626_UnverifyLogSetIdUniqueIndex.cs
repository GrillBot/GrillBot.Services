using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserMeasuresService.Core.Migrations
{
    /// <inheritdoc />
    public partial class UnverifyLogSetIdUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Unverifies_LogSetId",
                table: "Unverifies",
                column: "LogSetId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Unverifies_LogSetId",
                table: "Unverifies");
        }
    }
}

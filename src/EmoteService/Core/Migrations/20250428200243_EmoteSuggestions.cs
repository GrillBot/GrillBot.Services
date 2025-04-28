using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmoteService.Core.Migrations
{
    /// <inheritdoc />
    public partial class EmoteSuggestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "suggestions");

            migrationBuilder.CreateTable(
                name: "EmoteSuggestions",
                schema: "suggestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FromUserId = table.Column<decimal>(type: "numeric", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    SuggestedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Image = table.Column<byte[]>(type: "bytea", nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric", nullable: false),
                    SuggestionMessageId = table.Column<decimal>(type: "numeric", nullable: false),
                    ApprovedForVote = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmoteSuggestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "numeric", nullable: false),
                    SuggestionChannelId = table.Column<decimal>(type: "numeric", nullable: false),
                    VoteChannelId = table.Column<decimal>(type: "numeric", nullable: false),
                    VoteTime = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "EmoteVoteSessions",
                schema: "suggestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VoteStartedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ExpectedVoteEndAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmoteVoteSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmoteVoteSessions_EmoteSuggestions_Id",
                        column: x => x.Id,
                        principalSchema: "suggestions",
                        principalTable: "EmoteSuggestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmoteUserVotes",
                schema: "suggestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VoteSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsApproved = table.Column<bool>(type: "boolean", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmoteUserVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmoteUserVotes_EmoteVoteSessions_VoteSessionId",
                        column: x => x.VoteSessionId,
                        principalSchema: "suggestions",
                        principalTable: "EmoteVoteSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmoteUserVotes_VoteSessionId",
                schema: "suggestions",
                table: "EmoteUserVotes",
                column: "VoteSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmoteUserVotes",
                schema: "suggestions");

            migrationBuilder.DropTable(
                name: "Guilds");

            migrationBuilder.DropTable(
                name: "EmoteVoteSessions",
                schema: "suggestions");

            migrationBuilder.DropTable(
                name: "EmoteSuggestions",
                schema: "suggestions");
        }
    }
}

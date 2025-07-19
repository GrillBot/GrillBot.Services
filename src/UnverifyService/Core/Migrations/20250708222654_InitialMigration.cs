using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UnverifyService.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "logs");

            migrationBuilder.CreateTable(
                name: "Guilds",
                columns: table => new
                {
                    GuildId = table.Column<decimal>(type: "numeric", nullable: false),
                    MuteRoleId = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Guilds", x => x.GuildId);
                });

            migrationBuilder.CreateTable(
                name: "LogItems",
                schema: "logs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OperationType = table.Column<int>(type: "integer", nullable: false),
                    GuildId = table.Column<decimal>(type: "numeric", nullable: false),
                    FromUserId = table.Column<decimal>(type: "numeric", nullable: false),
                    ToUserId = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SelfUnverifyKeepables",
                columns: table => new
                {
                    Group = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelfUnverifyKeepables", x => new { x.Group, x.Name });
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "numeric", nullable: false),
                    SelfUnverifyMinimalTime = table.Column<TimeSpan>(type: "interval", nullable: true),
                    IsBotAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    IsBot = table.Column<bool>(type: "boolean", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ActiveUnverifies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LogSetId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveUnverifies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActiveUnverifies_LogItems_LogSetId",
                        column: x => x.LogSetId,
                        principalSchema: "logs",
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnverifyLogRemoveOperation",
                schema: "logs",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsFromWeb = table.Column<bool>(type: "boolean", nullable: false),
                    Language = table.Column<string>(type: "character varying(85)", maxLength: 85, nullable: true),
                    Force = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnverifyLogRemoveOperation", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_UnverifyLogRemoveOperation_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalSchema: "logs",
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnverifyLogSetOperation",
                schema: "logs",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    StartAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    Language = table.Column<string>(type: "character varying(85)", maxLength: 85, nullable: false),
                    KeepMutedRole = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnverifyLogSetOperation", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_UnverifyLogSetOperation_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalSchema: "logs",
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnverifyLogUpdateOperation",
                schema: "logs",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    NewStartAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NewEndAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnverifyLogUpdateOperation", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_UnverifyLogUpdateOperation_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalSchema: "logs",
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnverifyLogRemoveChannel",
                schema: "logs",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<decimal>(type: "numeric", nullable: false),
                    AllowValue = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    DenyValue = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnverifyLogRemoveChannel", x => new { x.LogItemId, x.ChannelId });
                    table.ForeignKey(
                        name: "FK_UnverifyLogRemoveChannel_UnverifyLogRemoveOperation_LogItem~",
                        column: x => x.LogItemId,
                        principalSchema: "logs",
                        principalTable: "UnverifyLogRemoveOperation",
                        principalColumn: "LogItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnverifyLogRemoveRole",
                schema: "logs",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnverifyLogRemoveRole", x => new { x.LogItemId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UnverifyLogRemoveRole_UnverifyLogRemoveOperation_LogItemId",
                        column: x => x.LogItemId,
                        principalSchema: "logs",
                        principalTable: "UnverifyLogRemoveOperation",
                        principalColumn: "LogItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnverifyLogSetChannel",
                schema: "logs",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelId = table.Column<decimal>(type: "numeric", nullable: false),
                    AllowValue = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    DenyValue = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnverifyLogSetChannel", x => new { x.LogItemId, x.ChannelId });
                    table.ForeignKey(
                        name: "FK_UnverifyLogSetChannel_UnverifyLogSetOperation_LogItemId",
                        column: x => x.LogItemId,
                        principalSchema: "logs",
                        principalTable: "UnverifyLogSetOperation",
                        principalColumn: "LogItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnverifyLogSetRole",
                schema: "logs",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<decimal>(type: "numeric", nullable: false),
                    IsKept = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnverifyLogSetRole", x => new { x.LogItemId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UnverifyLogSetRole_UnverifyLogSetOperation_LogItemId",
                        column: x => x.LogItemId,
                        principalSchema: "logs",
                        principalTable: "UnverifyLogSetOperation",
                        principalColumn: "LogItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveUnverifies_LogSetId",
                table: "ActiveUnverifies",
                column: "LogSetId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveUnverifies");

            migrationBuilder.DropTable(
                name: "Guilds");

            migrationBuilder.DropTable(
                name: "SelfUnverifyKeepables");

            migrationBuilder.DropTable(
                name: "UnverifyLogRemoveChannel",
                schema: "logs");

            migrationBuilder.DropTable(
                name: "UnverifyLogRemoveRole",
                schema: "logs");

            migrationBuilder.DropTable(
                name: "UnverifyLogSetChannel",
                schema: "logs");

            migrationBuilder.DropTable(
                name: "UnverifyLogSetRole",
                schema: "logs");

            migrationBuilder.DropTable(
                name: "UnverifyLogUpdateOperation",
                schema: "logs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "UnverifyLogRemoveOperation",
                schema: "logs");

            migrationBuilder.DropTable(
                name: "UnverifyLogSetOperation",
                schema: "logs");

            migrationBuilder.DropTable(
                name: "LogItems",
                schema: "logs");
        }
    }
}

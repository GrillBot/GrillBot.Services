using System;
using System.Collections.Generic;
using AuditLogService.Core.Entity;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuditLogService.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildInfoItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DefaultMessageNotifications = table.Column<int>(type: "integer", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    VanityUrl = table.Column<string>(type: "text", nullable: true),
                    BannerId = table.Column<string>(type: "text", nullable: true),
                    DiscoverySplashId = table.Column<string>(type: "text", nullable: true),
                    SplashId = table.Column<string>(type: "text", nullable: true),
                    IconId = table.Column<string>(type: "text", nullable: true),
                    IconData = table.Column<byte[]>(type: "bytea", nullable: true),
                    VoiceRegionId = table.Column<string>(type: "text", nullable: true),
                    OwnerId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    PublicUpdatesChannelId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    RulesChannelId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    SystemChannelId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    AfkChannelId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    AfkTimeout = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MfaLevel = table.Column<int>(type: "integer", nullable: false),
                    VerificationLevel = table.Column<int>(type: "integer", nullable: false),
                    ExplicitContentFilter = table.Column<int>(type: "integer", nullable: false),
                    Features = table.Column<long>(type: "bigint", nullable: false),
                    PremiumTier = table.Column<int>(type: "integer", nullable: false),
                    SystemChannelFlags = table.Column<int>(type: "integer", nullable: false),
                    NsfwLevel = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildInfoItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChannelInfoItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelName = table.Column<string>(type: "text", nullable: false),
                    SlowMode = table.Column<int>(type: "integer", nullable: true),
                    ChannelType = table.Column<int>(type: "integer", nullable: false),
                    IsNsfw = table.Column<bool>(type: "boolean", nullable: false),
                    Bitrate = table.Column<int>(type: "integer", nullable: true),
                    Topic = table.Column<string>(type: "text", nullable: true),
                    Position = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelInfoItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LogItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GuildId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    UserId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    ChannelId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    DiscordId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MemberInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    Nickname = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    IsMuted = table.Column<bool>(type: "boolean", nullable: true),
                    IsDeaf = table.Column<bool>(type: "boolean", nullable: true),
                    SelfUnverifyMinimalTime = table.Column<string>(type: "text", nullable: true),
                    Flags = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OverwriteInfoItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Target = table.Column<int>(type: "integer", nullable: false),
                    TargetId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    AllowValue = table.Column<string>(type: "text", nullable: false),
                    DenyValue = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OverwriteInfoItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ThreadInfoItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ThreadName = table.Column<string>(type: "text", nullable: false),
                    SlowMode = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    IsArchived = table.Column<bool>(type: "boolean", nullable: false),
                    ArchiveDuration = table.Column<int>(type: "integer", nullable: false),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                    Tags = table.Column<List<string>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreadInfoItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiRequests",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ControllerName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    ActionName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    StartAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Method = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    TemplatePath = table.Column<string>(type: "text", nullable: false),
                    Path = table.Column<string>(type: "text", nullable: false),
                    Parameters = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: false),
                    Language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ApiGroupName = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    Headers = table.Column<Dictionary<string, string>>(type: "jsonb", nullable: false),
                    Identification = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Ip = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiRequests", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_ApiRequests_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeletedEmotes",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    EmoteId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    EmoteName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeletedEmotes", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_DeletedEmotes_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Filename = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Extension = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Size = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GuildUpdatedItems",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    BeforeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AfterId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildUpdatedItems", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_GuildUpdatedItems_GuildInfoItems_AfterId",
                        column: x => x.AfterId,
                        principalTable: "GuildInfoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildUpdatedItems_GuildInfoItems_BeforeId",
                        column: x => x.BeforeId,
                        principalTable: "GuildInfoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GuildUpdatedItems_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelCreatedItems",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelInfoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelCreatedItems", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_ChannelCreatedItems_ChannelInfoItems_ChannelInfoId",
                        column: x => x.ChannelInfoId,
                        principalTable: "ChannelInfoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelCreatedItems_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelDeletedItems",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChannelInfoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelDeletedItems", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_ChannelDeletedItems_ChannelInfoItems_ChannelInfoId",
                        column: x => x.ChannelInfoId,
                        principalTable: "ChannelInfoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelDeletedItems_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChannelUpdatedItems",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    BeforeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AfterId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelUpdatedItems", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_ChannelUpdatedItems_ChannelInfoItems_AfterId",
                        column: x => x.AfterId,
                        principalTable: "ChannelInfoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelUpdatedItems_ChannelInfoItems_BeforeId",
                        column: x => x.BeforeId,
                        principalTable: "ChannelInfoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelUpdatedItems_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InteractionCommands",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    ModuleName = table.Column<string>(type: "text", nullable: false),
                    MethodName = table.Column<string>(type: "text", nullable: false),
                    Parameters = table.Column<List<InteractionCommandParameter>>(type: "jsonb", nullable: false),
                    HasResponded = table.Column<bool>(type: "boolean", nullable: false),
                    IsValidToken = table.Column<bool>(type: "boolean", nullable: false),
                    IsSuccess = table.Column<bool>(type: "boolean", nullable: false),
                    CommandError = table.Column<int>(type: "integer", nullable: true),
                    ErrorReason = table.Column<string>(type: "text", nullable: true),
                    Duration = table.Column<int>(type: "integer", nullable: false),
                    Exception = table.Column<string>(type: "text", nullable: true),
                    Locale = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InteractionCommands", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_InteractionCommands_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobExecutions",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobName = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Result = table.Column<string>(type: "text", nullable: false),
                    StartAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WasError = table.Column<bool>(type: "boolean", nullable: false),
                    StartUserId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobExecutions", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_JobExecutions_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LogMessages",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    Severity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogMessages", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_LogMessages_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberRoleUpdatedItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    RoleId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    RoleName = table.Column<string>(type: "text", nullable: false),
                    RoleColor = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    IsAdded = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberRoleUpdatedItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberRoleUpdatedItems_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageDeletedItems",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    MessageCreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageDeletedItems", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_MessageDeletedItems_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageEditedItems",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    JumpUrl = table.Column<string>(type: "text", nullable: false),
                    ContentBefore = table.Column<string>(type: "text", nullable: false),
                    ContentAfter = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageEditedItems", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_MessageEditedItems_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Unbans",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Unbans", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_Unbans_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserJoinedItems",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserJoinedItems", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_UserJoinedItems_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLeftItems",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    MemberCount = table.Column<int>(type: "integer", nullable: false),
                    IsBan = table.Column<bool>(type: "boolean", nullable: false),
                    BanReason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLeftItems", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_UserLeftItems_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberUpdatedItems",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    BeforeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AfterId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberUpdatedItems", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_MemberUpdatedItems_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberUpdatedItems_MemberInfos_AfterId",
                        column: x => x.AfterId,
                        principalTable: "MemberInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberUpdatedItems_MemberInfos_BeforeId",
                        column: x => x.BeforeId,
                        principalTable: "MemberInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OverwriteCreatedItems",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    OverwriteInfoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OverwriteCreatedItems", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_OverwriteCreatedItems_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OverwriteCreatedItems_OverwriteInfoItems_OverwriteInfoId",
                        column: x => x.OverwriteInfoId,
                        principalTable: "OverwriteInfoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OverwriteDeletedItems",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    OverwriteInfoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OverwriteDeletedItems", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_OverwriteDeletedItems_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OverwriteDeletedItems_OverwriteInfoItems_OverwriteInfoId",
                        column: x => x.OverwriteInfoId,
                        principalTable: "OverwriteInfoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OverwriteUpdatedItems",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    BeforeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AfterId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OverwriteUpdatedItems", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_OverwriteUpdatedItems_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OverwriteUpdatedItems_OverwriteInfoItems_AfterId",
                        column: x => x.AfterId,
                        principalTable: "OverwriteInfoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OverwriteUpdatedItems_OverwriteInfoItems_BeforeId",
                        column: x => x.BeforeId,
                        principalTable: "OverwriteInfoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThreadDeletedItems",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    ThreadInfoId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreadDeletedItems", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_ThreadDeletedItems_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThreadDeletedItems_ThreadInfoItems_ThreadInfoId",
                        column: x => x.ThreadInfoId,
                        principalTable: "ThreadInfoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThreadUpdatedItems",
                columns: table => new
                {
                    LogItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    BeforeId = table.Column<Guid>(type: "uuid", nullable: false),
                    AfterId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThreadUpdatedItems", x => x.LogItemId);
                    table.ForeignKey(
                        name: "FK_ThreadUpdatedItems_LogItems_LogItemId",
                        column: x => x.LogItemId,
                        principalTable: "LogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThreadUpdatedItems_ThreadInfoItems_AfterId",
                        column: x => x.AfterId,
                        principalTable: "ThreadInfoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ThreadUpdatedItems_ThreadInfoItems_BeforeId",
                        column: x => x.BeforeId,
                        principalTable: "ThreadInfoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmbedInfoItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MessageDeletedId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Type = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    ImageInfo = table.Column<string>(type: "text", nullable: true),
                    VideoInfo = table.Column<string>(type: "text", nullable: true),
                    AuthorName = table.Column<string>(type: "text", nullable: true),
                    ContainsFooter = table.Column<bool>(type: "boolean", nullable: false),
                    ProviderName = table.Column<string>(type: "text", nullable: true),
                    ThumbnailInfo = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmbedInfoItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmbedInfoItems_MessageDeletedItems_MessageDeletedId",
                        column: x => x.MessageDeletedId,
                        principalTable: "MessageDeletedItems",
                        principalColumn: "LogItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmbedFields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    EmbedInfoId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Value = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    Inline = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmbedFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmbedFields_EmbedInfoItems_EmbedInfoId",
                        column: x => x.EmbedInfoId,
                        principalTable: "EmbedInfoItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmbedFields_EmbedInfoId",
                table: "EmbedFields",
                column: "EmbedInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_EmbedInfoItems_MessageDeletedId",
                table: "EmbedInfoItems",
                column: "MessageDeletedId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_LogItemId",
                table: "Files",
                column: "LogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildUpdatedItems_AfterId",
                table: "GuildUpdatedItems",
                column: "AfterId");

            migrationBuilder.CreateIndex(
                name: "IX_GuildUpdatedItems_BeforeId",
                table: "GuildUpdatedItems",
                column: "BeforeId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelCreatedItems_ChannelInfoId",
                table: "ChannelCreatedItems",
                column: "ChannelInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelDeletedItems_ChannelInfoId",
                table: "ChannelDeletedItems",
                column: "ChannelInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelUpdatedItems_AfterId",
                table: "ChannelUpdatedItems",
                column: "AfterId");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelUpdatedItems_BeforeId",
                table: "ChannelUpdatedItems",
                column: "BeforeId");

            migrationBuilder.CreateIndex(
                name: "IX_LogItem_CreatedAt",
                table: "LogItems",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MemberRoleUpdatedItems_LogItemId",
                table: "MemberRoleUpdatedItems",
                column: "LogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberUpdatedItems_AfterId",
                table: "MemberUpdatedItems",
                column: "AfterId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberUpdatedItems_BeforeId",
                table: "MemberUpdatedItems",
                column: "BeforeId");

            migrationBuilder.CreateIndex(
                name: "IX_OverwriteCreatedItems_OverwriteInfoId",
                table: "OverwriteCreatedItems",
                column: "OverwriteInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_OverwriteDeletedItems_OverwriteInfoId",
                table: "OverwriteDeletedItems",
                column: "OverwriteInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_OverwriteUpdatedItems_AfterId",
                table: "OverwriteUpdatedItems",
                column: "AfterId");

            migrationBuilder.CreateIndex(
                name: "IX_OverwriteUpdatedItems_BeforeId",
                table: "OverwriteUpdatedItems",
                column: "BeforeId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadDeletedItems_ThreadInfoId",
                table: "ThreadDeletedItems",
                column: "ThreadInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadUpdatedItems_AfterId",
                table: "ThreadUpdatedItems",
                column: "AfterId");

            migrationBuilder.CreateIndex(
                name: "IX_ThreadUpdatedItems_BeforeId",
                table: "ThreadUpdatedItems",
                column: "BeforeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiRequests");

            migrationBuilder.DropTable(
                name: "DeletedEmotes");

            migrationBuilder.DropTable(
                name: "EmbedFields");

            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropTable(
                name: "GuildUpdatedItems");

            migrationBuilder.DropTable(
                name: "ChannelCreatedItems");

            migrationBuilder.DropTable(
                name: "ChannelDeletedItems");

            migrationBuilder.DropTable(
                name: "ChannelUpdatedItems");

            migrationBuilder.DropTable(
                name: "InteractionCommands");

            migrationBuilder.DropTable(
                name: "JobExecutions");

            migrationBuilder.DropTable(
                name: "LogMessages");

            migrationBuilder.DropTable(
                name: "MemberRoleUpdatedItems");

            migrationBuilder.DropTable(
                name: "MemberUpdatedItems");

            migrationBuilder.DropTable(
                name: "MessageEditedItems");

            migrationBuilder.DropTable(
                name: "OverwriteCreatedItems");

            migrationBuilder.DropTable(
                name: "OverwriteDeletedItems");

            migrationBuilder.DropTable(
                name: "OverwriteUpdatedItems");

            migrationBuilder.DropTable(
                name: "ThreadDeletedItems");

            migrationBuilder.DropTable(
                name: "ThreadUpdatedItems");

            migrationBuilder.DropTable(
                name: "Unbans");

            migrationBuilder.DropTable(
                name: "UserJoinedItems");

            migrationBuilder.DropTable(
                name: "UserLeftItems");

            migrationBuilder.DropTable(
                name: "EmbedInfoItems");

            migrationBuilder.DropTable(
                name: "GuildInfoItems");

            migrationBuilder.DropTable(
                name: "ChannelInfoItems");

            migrationBuilder.DropTable(
                name: "MemberInfos");

            migrationBuilder.DropTable(
                name: "OverwriteInfoItems");

            migrationBuilder.DropTable(
                name: "ThreadInfoItems");

            migrationBuilder.DropTable(
                name: "MessageDeletedItems");

            migrationBuilder.DropTable(
                name: "LogItems");
        }
    }
}

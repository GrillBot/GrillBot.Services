using AuditLogService.Core.Enums;

namespace AuditLogService.Models.Events.Create;

public class LogRequest
{
    public DateTime CreatedAtUtc { get; set; }
    public string? GuildId { get; set; }
    public string? UserId { get; set; }
    public string? ChannelId { get; set; }
    public string? DiscordId { get; set; }
    public LogType Type { get; set; }

    public List<FileRequest> Files { get; set; } = [];

    public ApiRequestRequest? ApiRequest { get; set; }
    public LogMessageRequest? LogMessage { get; set; }
    public DeletedEmoteRequest? DeletedEmote { get; set; }
    public UnbanRequest? Unban { get; set; }
    public JobExecutionRequest? JobExecution { get; set; }
    public ChannelInfoRequest? ChannelInfo { get; set; }
    public DiffRequest<ChannelInfoRequest>? ChannelUpdated { get; set; }
    public DiffRequest<GuildInfoRequest>? GuildUpdated { get; set; }
    public MessageDeletedRequest? MessageDeleted { get; set; }
    public MessageEditedRequest? MessageEdited { get; set; }
    public UserJoinedRequest? UserJoined { get; set; }
    public UserLeftRequest? UserLeft { get; set; }
    public InteractionCommandRequest? InteractionCommand { get; set; }
    public ThreadInfoRequest? ThreadInfo { get; set; }
    public DiffRequest<ThreadInfoRequest>? ThreadUpdated { get; set; }
    public MemberUpdatedRequest? MemberUpdated { get; set; }
    public RoleDeletedRequest? RoleDeleted { get; set; }

    public LogRequest()
    {
    }

    public LogRequest(LogType type, DateTime createdAtUtc, string? guildId = null, string? userId = null, string? channelId = null, string? discordId = null)
    {
        Type = type;
        CreatedAtUtc = createdAtUtc;
        GuildId = guildId;
        UserId = userId;
        ChannelId = channelId;
        DiscordId = discordId;
    }
}

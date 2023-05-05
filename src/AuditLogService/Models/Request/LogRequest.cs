using System.ComponentModel.DataAnnotations;
using AuditLogService.Core.Enums;
using GrillBot.Core.Validation;

namespace AuditLogService.Models.Request;

public class LogRequest : IValidatableObject
{
    public DateTime? CreatedAt { get; set; }

    [StringLength(32)]
    [DiscordId]
    public string? GuildId { get; set; }

    [StringLength(32)]
    [DiscordId]
    public string? UserId { get; set; }

    [StringLength(32)]
    [DiscordId]
    public string? ChannelId { get; set; }

    public LogType Type { get; set; }

    public List<FileRequest> Files { get; set; } = new();

    public ApiRequestRequest? ApiRequest { get; set; }
    public LogMessageRequest? LogMessage { get; set; }
    public DeletedEmoteRequest? DeletedEmote { get; set; }
    public UnbanRequest? Unban { get; set; }
    public JobExecutionRequest? JobExecution { get; set; }
    public ChannelInfoRequest? ChannelInfo { get; set; }
    public DiffRequest<ChannelInfoRequest>? ChannelUpdated { get; set; }
    public DiffRequest<GuildInfoRequest>? GuildUpdated { get; set; }
    public List<MemberRoleUpdated>? RoleUpdates { get; set; }
    public MessageDeletedRequest? MessageDeleted { get; set; }
    public MessageEditedRequest? MessageEdited { get; set; }
    public OverwriteInfoRequest? OverwriteInfo { get; set; }
    public DiffRequest<OverwriteInfoRequest>? OverwriteUpdated { get; set; }
    public UserJoinedRequest? UserJoined { get; set; }
    public UserLeftRequest? UserLeft { get; set; }
    public InteractionCommandRequest? InteractionCommand { get; set; }
    public ThreadInfoRequest? ThreadInfo { get; set; }
    public DiffRequest<ThreadInfoRequest>? ThreadUpdated { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CreatedAt.HasValue && CreatedAt.Value.Kind != DateTimeKind.Utc)
            yield return new ValidationResult("Only UTC value is allowed.", new[] { nameof(CreatedAt) });

        // <Type, Object>
        var requiredBindings = new Dictionary<LogType, object?>()
        {
            { LogType.Api, ApiRequest },
            { LogType.Info, LogMessage },
            { LogType.Warning, LogMessage },
            { LogType.Error, LogMessage },
            { LogType.EmoteDeleted, DeletedEmote },
            { LogType.Unban, Unban },
            { LogType.JobCompleted, JobExecution },
            { LogType.ChannelCreated, ChannelInfo },
            { LogType.ChannelDeleted, ChannelInfo },
            { LogType.ChannelUpdated, ChannelUpdated },
            { LogType.GuildUpdated, GuildUpdated },
            { LogType.MemberRoleUpdated, RoleUpdates },
            { LogType.MessageDeleted, MessageDeleted },
            { LogType.MessageEdited, MessageEdited },
            { LogType.OverwriteCreated, OverwriteInfo },
            { LogType.OverwriteDeleted, OverwriteInfo },
            { LogType.OverwriteUpdated, OverwriteUpdated },
            { LogType.UserJoined, UserJoined },
            { LogType.UserLeft, UserLeft },
            { LogType.InteractionCommand, InteractionCommand },
            { LogType.ThreadDeleted, ThreadInfo },
            { LogType.ThreadUpdated, ThreadUpdated }
        };

        if (requiredBindings.TryGetValue(Type, out var requestData) && requestData is null)
            yield return new ValidationResult($"Missing data for type {Type}");

        if (requiredBindings.Values.Count(o => o is not null) > 1)
            yield return new ValidationResult("Only one property with data can be setted.");
    }
}

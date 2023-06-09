using System.ComponentModel.DataAnnotations;
using AuditLogService.Core.Enums;
using AuditLogService.Validators;
using GrillBot.Core.Validation;

namespace AuditLogService.Models.Request.CreateItems;

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
    
    [StringLength(32)]
    [DiscordId]
    public string? DiscordId { get; set; }

    [Required]
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
    public MessageDeletedRequest? MessageDeleted { get; set; }
    public MessageEditedRequest? MessageEdited { get; set; }
    public UserJoinedRequest? UserJoined { get; set; }
    public UserLeftRequest? UserLeft { get; set; }
    public InteractionCommandRequest? InteractionCommand { get; set; }
    public ThreadInfoRequest? ThreadInfo { get; set; }
    public DiffRequest<ThreadInfoRequest>? ThreadUpdated { get; set; }
    public MemberUpdatedRequest? MemberUpdated { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var validator = new LogRequestValidator();
        return validator.Validate(this, validationContext);
    }
}

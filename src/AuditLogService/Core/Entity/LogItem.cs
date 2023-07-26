using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Core.Entity;

[Index(nameof(CreatedAt), Name = "IX_LogItem_CreatedAt")]
[Index(nameof(GuildId))]
[Index(nameof(LogDate))]
public class LogItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }

    [StringLength(32)]
    public string? GuildId { get; set; }

    [StringLength(32)]
    public string? UserId { get; set; }

    [StringLength(32)]
    public string? ChannelId { get; set; }

    [StringLength(32)]
    public string? DiscordId { get; set; }

    public LogType Type { get; set; }

    public ISet<File> Files { get; set; }

    public bool IsDeleted { get; set; }
    public bool IsPendingProcess { get; set; }

    public DateOnly LogDate { get; set; }

    #region Data

    public ApiRequest? ApiRequest { get; set; }
    public LogMessage? LogMessage { get; set; }
    public DeletedEmote? DeletedEmote { get; set; }
    public Unban? Unban { get; set; }
    public JobExecution? Job { get; set; }
    public ChannelCreated? ChannelCreated { get; set; }
    public ChannelDeleted? ChannelDeleted { get; set; }
    public ChannelUpdated? ChannelUpdated { get; set; }
    public GuildUpdated? GuildUpdated { get; set; }
    public ISet<MemberRoleUpdated>? MemberRolesUpdated { get; set; }
    public MessageDeleted? MessageDeleted { get; set; }
    public MessageEdited? MessageEdited { get; set; }
    public OverwriteCreated? OverwriteCreated { get; set; }
    public OverwriteUpdated? OverwriteUpdated { get; set; }
    public OverwriteDeleted? OverwriteDeleted { get; set; }
    public UserJoined? UserJoined { get; set; }
    public UserLeft? UserLeft { get; set; }
    public InteractionCommand? InteractionCommand { get; set; }
    public ThreadDeleted? ThreadDeleted { get; set; }
    public ThreadUpdated? ThreadUpdated { get; set; }
    public MemberUpdated? MemberUpdated { get; set; }

    #endregion

    #region HelperProperties

    [NotMapped]
    public bool CanCreate { get; set; }

    [NotMapped]
    public List<LogItem> MergedItems { get; set; } = new();
    
    #endregion

    public LogItem()
    {
        Files = new HashSet<File>();
    }
}

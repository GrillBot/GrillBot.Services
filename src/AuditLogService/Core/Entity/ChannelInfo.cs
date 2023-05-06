using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Discord;

namespace AuditLogService.Core.Entity;

public class ChannelInfo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public Guid Id { get; set; }

    public string? ChannelName { get; set; } = null!;
    public int? SlowMode { get; set; }
    public ChannelType? ChannelType { get; set; }
    public bool? IsNsfw { get; set; }
    public int? Bitrate { get; set; }
    public string? Topic { get; set; }
    public int Position { get; set; }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemindService.Core.Entity;

public class RemindMessage
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [StringLength(32)]
    public string FromUserId { get; set; } = null!;

    [StringLength(32)]
    public string ToUserId { get; set; } = null!;

    public DateTime NotifyAtUtc { get; set; }

    public string Message { get; set; } = null!;

    public int PostponeCount { get; set; }

    [StringLength(32)]
    public string? NotificationMessageId { get; set; }

    [StringLength(32)]
    public string CommandMessageId { get; set; } = null!;

    [StringLength(32)]
    public string Language { get; set; } = null!;

    public bool IsSendInProgress { get; set; }
}

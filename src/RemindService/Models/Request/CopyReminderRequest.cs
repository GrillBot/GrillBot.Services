using GrillBot.Core.Validation;
using System.ComponentModel.DataAnnotations;

namespace RemindService.Models.Request;

public class CopyReminderRequest
{
    public int RemindId { get; set; }

    [DiscordId]
    [StringLength(32)]
    public string ToUserId { get; set; } = null!;

    [StringLength(32)]
    public string Language { get; set; } = null!;
}

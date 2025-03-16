using GrillBot.Core.Validation;
using System.ComponentModel.DataAnnotations;

namespace RemindService.Models.Request;

public class CancelReminderRequest
{
    public int RemindId { get; set; }

    /// <summary>
    /// ID of user who executes cancellation.
    /// </summary>
    [DiscordId]
    [StringLength(32)]
    public string ExecutingUserId { get; set; } = null!;

    public bool IsAdminExecution { get; set; }

    public bool NotifyUser { get; set; }
}

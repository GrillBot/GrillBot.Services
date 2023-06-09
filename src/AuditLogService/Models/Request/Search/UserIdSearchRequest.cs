using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;

namespace AuditLogService.Models.Request.Search;

public class UserIdSearchRequest : IAdvancedSearchRequest
{
    [DiscordId]
    [StringLength(32)]
    public string? UserId { get; set; }

    public bool IsSet()
        => !string.IsNullOrEmpty(UserId);
}

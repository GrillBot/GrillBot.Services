using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;

namespace AuditLogService.Models.Request.Search;

public class MessageDeletedSearchRequest : IAdvancedSearchRequest
{
    public bool? ContainsEmbed { get; set; }
    public string? ContentContains { get; set; }

    [DiscordId]
    [StringLength(32)]
    public string? AuthorId { get; set; }

    public bool IsSet()
        => ContainsEmbed is not null || !string.IsNullOrEmpty(ContentContains) || !string.IsNullOrEmpty(AuthorId);
}

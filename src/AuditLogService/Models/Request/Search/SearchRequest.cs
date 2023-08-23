using System.ComponentModel.DataAnnotations;
using AuditLogService.Core.Enums;
using AuditLogService.Validators;
using GrillBot.Core.Models;
using GrillBot.Core.Models.Pagination;
using GrillBot.Core.Validation;

namespace AuditLogService.Models.Request.Search;

public class SearchRequest : IValidatableObject
{
    [DiscordId]
    [StringLength(32)]
    public string? GuildId { get; set; }

    [DiscordId]
    public List<string> UserIds { get; set; } = new();

    [DiscordId]
    [StringLength(32)]
    public string? ChannelId { get; set; }

    public List<LogType> ShowTypes { get; set; } = new();
    public List<LogType> IgnoreTypes { get; set; } = new();
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public bool OnlyWithFiles { get; set; }
    public List<Guid>? Ids { get; set; }

    public AdvancedSearchRequest? AdvancedSearch { get; set; }

    public SortParameters Sort { get; set; } = new() { Descending = true, OrderBy = "CreatedAt" };
    public PaginatedParams Pagination { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        => new SearchRequestValidator().Validate(this, validationContext);

    public bool IsAdvancedFilterSet(LogType type)
    {
        if (!ShowTypes.Contains(type) || AdvancedSearch is null)
            return false;

        IAdvancedSearchRequest? advancedSearchRequest = type switch
        {
            LogType.Info => AdvancedSearch.Info,
            LogType.Warning => AdvancedSearch.Warning,
            LogType.Error => AdvancedSearch.Error,
            LogType.InteractionCommand => AdvancedSearch.Interaction,
            LogType.JobCompleted => AdvancedSearch.Job,
            LogType.Api => AdvancedSearch.Api,
            LogType.OverwriteCreated => AdvancedSearch.OverwriteCreated,
            LogType.OverwriteDeleted => AdvancedSearch.OverwriteDeleted,
            LogType.OverwriteUpdated => AdvancedSearch.OverwriteUpdated,
            LogType.MemberUpdated => AdvancedSearch.MemberUpdated,
            LogType.MemberRoleUpdated => AdvancedSearch.MemberRolesUpdated,
            LogType.MessageDeleted => AdvancedSearch.MessageDeleted,
            _ => null
        };

        return advancedSearchRequest?.IsSet() == true;
    }

    public bool IsAnyAdvancedFilterSet()
        => ShowTypes.Count > 0 && AdvancedSearch is not null && Enum.GetValues<LogType>().Any(IsAdvancedFilterSet);
}

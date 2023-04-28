using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Database;
using GrillBot.Core.Models;
using GrillBot.Core.Models.Pagination;
using GrillBot.Core.Validation;
using PointsService.Core.Entity;

namespace PointsService.Models;

public class AdminListRequest : IValidatableObject, IQueryableModel<Transaction>
{
    [Required]
    public bool ShowMerged { get; set; }

    [StringLength(30)]
    [DiscordId]
    public string? GuildId { get; set; }

    [StringLength(30)]
    [DiscordId]
    public string? UserId { get; set; }

    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }

    public bool OnlyReactions { get; set; }
    public bool OnlyMessages { get; set; }

    [DiscordId]
    [StringLength(30)]
    public string? MessageId { get; set; }

    public PaginatedParams Pagination { get; set; } = new();
    public SortParameters? Sort { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CreatedFrom > CreatedTo)
            yield return new ValidationResult("Invalid interval From-To", new[] { nameof(CreatedFrom), nameof(CreatedTo) });
    }

    public IQueryable<Transaction> SetQuery(IQueryable<Transaction> query)
    {
        query = ShowMerged ? query.Where(o => o.MergedCount > 0) : query.Where(o => o.MergedCount == 0);

        if (!string.IsNullOrEmpty(GuildId))
            query = query.Where(o => o.GuildId == GuildId);
        if (!string.IsNullOrEmpty(UserId))
            query = query.Where(o => o.UserId == UserId);
        if (CreatedFrom.HasValue)
            query = query.Where(o => o.CreatedAt >= CreatedFrom.Value.ToUniversalTime());
        if (CreatedTo.HasValue)
            query = query.Where(o => o.CreatedAt < CreatedTo.Value.ToUniversalTime());

        if (OnlyMessages)
            query = query.Where(o => o.ReactionId == "");
        if (OnlyReactions)
            query = query.Where(o => o.ReactionId != "");
        if (!ShowMerged && !string.IsNullOrEmpty(MessageId))
            query = query.Where(o => o.MessageId == MessageId);
        return query;
    }

    public IQueryable<Transaction> SetIncludes(IQueryable<Transaction> query) => query;

    public IQueryable<Transaction> SetSort(IQueryable<Transaction> query)
    {
        if (Sort is null)
            return query;
        
        return Sort.OrderBy switch
        {
            "Value" => Sort.Descending ? query.OrderByDescending(o => o.Value).ThenByDescending(o => o.CreatedAt) : query.OrderBy(o => o.Value).ThenBy(o => o.CreatedAt),
            _ => Sort.Descending ? query.OrderByDescending(o => o.CreatedAt) : query.OrderBy(o => o.CreatedAt)
        };
    }
}

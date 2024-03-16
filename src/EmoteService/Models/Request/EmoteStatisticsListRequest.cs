using GrillBot.Core.Models;
using GrillBot.Core.Models.Pagination;
using GrillBot.Core.Validation;
using System.ComponentModel.DataAnnotations;

namespace EmoteService.Models.Request;

public class EmoteStatisticsListRequest
{
    public bool Unsupported { get; set; }

    [DiscordId]
    [StringLength(32)]
    public string? GuildId { get; set; }

    [DiscordId]
    [StringLength(32)]
    public string? UserId { get; set; }

    public int? UseCountFrom { get; set; }
    public int? UseCountTo { get; set; }
    public DateTime? FirstOccurenceFrom { get; set; }
    public DateTime? FirstOccurenceTo { get; set; }
    public DateTime? LastOccurenceFrom { get; set; }
    public DateTime? LastOccurenceTo { get; set; }

    public bool IgnoreAnimated { get; set; }

    [StringLength(128)]
    public string? EmoteName { get; set; }

    [StringLength(255)]
    public string? EmoteFullId { get; set; }

    public SortParameters Sort { get; set; } = new() { Descending = true, OrderBy = "UseCount" };
    public PaginatedParams Pagination { get; set; } = new();
}

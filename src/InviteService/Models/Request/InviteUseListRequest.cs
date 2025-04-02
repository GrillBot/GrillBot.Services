using GrillBot.Core.Models;
using GrillBot.Core.Models.Pagination;
using GrillBot.Core.Validation;
using System.ComponentModel.DataAnnotations;

namespace InviteService.Models.Request;

public class InviteUseListRequest
{
    [DiscordId]
    [StringLength(32)]
    public string GuildId { get; set; } = null!;

    [StringLength(10)]
    public string Code { get; set; } = null!;

    public DateTime? UsedFrom { get; set; }
    public DateTime? UsedTo { get; set; }

    /// <summary>
    /// Available: UsedAt,
    /// Default: UsedAt
    /// </summary>
    public SortParameters Sort { get; set; } = new() { OrderBy = "UsedAt" };

    public PaginatedParams Pagination { get; set; } = new();
}

using GrillBot.Core.Models;
using GrillBot.Core.Models.Pagination;
using GrillBot.Core.Validation;
using System.ComponentModel.DataAnnotations;

namespace EmoteService.Models.Request;

public class EmoteUserUsageListRequest
{
    [DiscordId]
    [StringLength(32)]
    public string GuildId { get; set; } = null!;

    [EmoteId]
    [StringLength(255)]
    public string EmoteId { get; set; } = null!;

    public SortParameters Sort { get; set; } = new() { Descending = true, OrderBy = "UseCount" };
    public PaginatedParams Pagination { get; set; } = new();
}

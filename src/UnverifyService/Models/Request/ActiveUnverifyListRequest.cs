using GrillBot.Core.Models;
using GrillBot.Core.Models.Pagination;

namespace UnverifyService.Models.Request;

public class ActiveUnverifyListRequest
{
    public string? GuildId { get; set; }

    /// <summary>
    /// Default: StartAt,
    /// PossibleValues: StartAt, EndAt
    /// </summary>
    public SortParameters Sort { get; set; } = new() { OrderBy = "StartAt" };
    public PaginatedParams Pagination { get; set; } = new();
}

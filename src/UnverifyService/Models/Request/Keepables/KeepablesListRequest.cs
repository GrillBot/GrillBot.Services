using GrillBot.Core.Models;
using GrillBot.Core.Models.Pagination;

namespace UnverifyService.Models.Request.Keepables;

public class KeepablesListRequest
{
    public string? Group { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }

    /// <summary>
    /// Possible values: Group, Name, Created
    /// Default: Group
    /// </summary>
    public SortParameters Sort { get; set; } = new() { OrderBy = "Group" };
    public PaginatedParams Pagination { get; set; } = new();
}

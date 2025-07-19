using GrillBot.Core.Models;
using GrillBot.Core.Models.Pagination;
using UnverifyService.Core.Enums;

namespace UnverifyService.Models.Request.Logs;

public class UnverifyLogListRequest
{
    public UnverifyOperationType? Operation { get; set; }
    public string? FromUserId { get; set; }
    public string? ToUserId { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public string? GuildId { get; set; }
    public Guid? ParentLogItemId { get; set; }

    /// <summary>
    /// Possible values: Crated, Operation
    /// Default: Created
    /// </summary>
    public SortParameters Sort { get; set; } = new() { OrderBy = "Created" };
    public PaginatedParams Pagination { get; set; } = new();
}

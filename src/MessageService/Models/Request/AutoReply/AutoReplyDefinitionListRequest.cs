using GrillBot.Core.Models;
using GrillBot.Core.Models.Pagination;

namespace MessageService.Models.Request.AutoReply;

public class AutoReplyDefinitionListRequest
{
    public string? TemplateContains { get; set; }
    public string? ReplyContains { get; set; }
    public bool HideDisabled { get; set; }

    public SortParameters Sort { get; set; } = new() { OrderBy = "Template" };
    public PaginatedParams Pagination { get; set; } = new();
}

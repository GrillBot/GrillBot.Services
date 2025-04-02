using GrillBot.Core.Models;
using GrillBot.Core.Models.Pagination;
using GrillBot.Core.Validation;
using System.ComponentModel.DataAnnotations;

namespace InviteService.Models.Request;

public class UserInviteUseListRequest
{
    [DiscordId]
    [StringLength(32)]
    public string UserId { get; set; } = null!;

    public SortParameters Sort { get; set; } = new();
    public PaginatedParams Pagination { get; set; } = new();
}

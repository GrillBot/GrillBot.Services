using GrillBot.Core.Models.Pagination;
using System.ComponentModel.DataAnnotations;

namespace PointsService.Models.Users;

public class UserListRequest
{
    [StringLength(30)]
    public string? GuildId { get; set; }

    public PaginatedParams Pagination { get; set; } = new();
}

using System.ComponentModel.DataAnnotations;

namespace PointsService.Core.Entity;

public class LeaderboardItem
{
    [StringLength(30)]
    public string GuildId { get; set; } = null!;

    [StringLength(30)]
    public string UserId { get; set; } = null!;

    public int YearBack { get; set; }
    public int MonthBack { get; set; }
    public int Today { get; set; }
    public int Total { get; set; }
}

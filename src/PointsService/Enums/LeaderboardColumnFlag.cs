namespace PointsService.Enums;

[Flags]
public enum LeaderboardColumnFlag
{
    None = 0,
    YearBack = 1,
    MonthBack = 2,
    Today = 4,
    Total = 8
}

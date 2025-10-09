namespace UserMeasuresService.Models.User;

public record UserInfo(
    Dictionary<string, int> WarningCount,
    Dictionary<string, int> UnverifyCount,
    Dictionary<string, int> TimeoutCount
);

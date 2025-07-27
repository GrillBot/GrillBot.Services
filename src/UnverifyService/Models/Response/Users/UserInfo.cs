namespace UnverifyService.Models.Response.Users;

public record UserInfo(
    TimeSpan? SelfUnverifyMinimalTime,
    Dictionary<string, UnverifyInfo> CurrentUnverifies,
    Dictionary<string, int> SelfUnverifyCount,
    Dictionary<string, int> UnverifyCount
);

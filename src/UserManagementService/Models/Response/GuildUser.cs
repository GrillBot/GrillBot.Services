using UnverifyService.Models.Response;

namespace UserManagementService.Models.Response;

public record GuildUser(
    string GuildId,
    string? CurrentNickname,
    List<string> NicknameHistory,
    UnverifyInfo? CurrentUnverify,
    int UnverifyCount,
    int SelfUnverifyCount,
    int TimeoutCount,
    int WarningCount
);

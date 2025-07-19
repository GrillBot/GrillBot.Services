using UnverifyService.Core.Enums;

namespace UnverifyService.Models.Response.Logs;

public record UnverifyLogItem(
    Guid Id,
    Guid? ParentItemId,
    UnverifyOperationType Type,
    string GuildId,
    string FromUserId,
    string ToUserId,
    DateTime CreatedAtUtc,
    object? Preview
);

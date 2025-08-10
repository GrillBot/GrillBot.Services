using UnverifyService.Core.Enums;

namespace UnverifyService.Models.Response.Logs.Detail;

public record UnverifyLogDetail(
    Guid Id,
    long LogNumber,
    UnverifyLogSimpleDetail? ParentInfo,
    UnverifyOperationType OperationType,
    string GuildId,
    string FromUserId,
    string ToUserId,
    DateTime CreatedAtUtc,
    object? Data
);

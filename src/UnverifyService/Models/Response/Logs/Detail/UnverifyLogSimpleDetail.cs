using UnverifyService.Core.Enums;

namespace UnverifyService.Models.Response.Logs.Detail;

public record UnverifyLogSimpleDetail(
    Guid Id,
    long LogNumber,
    string FromUserId,
    string ToUserId,
    DateTime CreatedAtUtc,
    UnverifyOperationType Type
);

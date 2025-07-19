using UnverifyService.Core.Enums;

namespace UnverifyService.Models.Response.Logs.Detail;

public record UnverifyLogSimpleDetail(
    Guid Id,
    string FromUserId,
    string ToUserId,
    DateTime CreatedAtUtc,
    UnverifyOperationType Type
);

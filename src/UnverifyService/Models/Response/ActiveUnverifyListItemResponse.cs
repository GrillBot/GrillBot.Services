namespace UnverifyService.Models.Response;

public record ActiveUnverifyListItemResponse(
    Guid Id,
    string GuildId,
    string FromUserId,
    string ToUserId,
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    bool IsSelfUnverify,
    string? Reason,
    string Language,
    int RemovedRolesCount,
    int KeepedRolesCount,
    int RemovedChannelsCount,
    int KeepedChannelsCount,
    bool IsReadOnly
);

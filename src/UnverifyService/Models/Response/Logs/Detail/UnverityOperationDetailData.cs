namespace UnverifyService.Models.Response.Logs.Detail;

public record UnverityOperationDetailData(
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    long DurationMilliseconds,
    string Reason,
    string Language,
    bool IsMutedRoleKeeped,
    List<string> RemovedRoles,
    List<string> KeepedRoles,
    List<ChannelDetailData> RemovedChannels,
    List<ChannelDetailData> KeepedChannels
);

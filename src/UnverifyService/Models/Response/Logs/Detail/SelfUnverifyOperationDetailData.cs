﻿namespace UnverifyService.Models.Response.Logs.Detail;

public record SelfUnverifyOperationDetailData(
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    long DurationMilliseconds,
    string Language,
    bool IsMutedRoleKeeped,
    List<string> RemovedRoles,
    List<string> KeepedRoles,
    List<ChannelDetailData> RemovedChannels,
    List<ChannelDetailData> KeepedChannels
);
namespace UnverifyService.Models.Response.Logs;

public record SelfUnverifyPreview(
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    int RemovedRolesCount,
    int KeepedRolesCount,
    int RemovedChnannelsCount,
    int KeepedChnannelsCount
);

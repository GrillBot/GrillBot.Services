namespace UnverifyService.Models.Response.Logs;

public record RemovePreview(
    int ReturnedRolesCount,
    int ReturnedChannelsCount,
    bool IsFromWeb,
    bool IsForcedRemoval
);

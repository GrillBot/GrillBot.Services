using GrillBot.Models;

namespace UnverifyService.Models.Response;

public record RemoveUnverifyResponse(
    LocalizedMessageContent Message,
    int ReturnedRolesCount,
    int ReturnedChannelsCount
);

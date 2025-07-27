namespace UnverifyService.Models.Response;

public record ScheduleUnverifyRemoveItem(
    ulong GuildId,
    ulong UserId,
    int RolesToReturnCount,
    int ChannelsToReturnCount
);

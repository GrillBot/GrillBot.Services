namespace UnverifyService.Models.Response;

public record ChannelOverride(
    string ChannelId,
    List<string> AllowValues,
    List<string> DenyValues
);

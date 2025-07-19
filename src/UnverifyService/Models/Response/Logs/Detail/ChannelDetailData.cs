namespace UnverifyService.Models.Response.Logs.Detail;

public record ChannelDetailData(
    string ChannelId,
    List<string> AllowValues,
    List<string> DenyValues
);

namespace UnverifyService.Models.Response.Logs.Detail;

public record AutoRemoveOperationDetailData(
    string Language,
    List<string> ReturnedRoles,
    List<ChannelDetailData> ReturnedChannels
);
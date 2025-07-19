namespace UnverifyService.Models.Response.Logs.Detail;

public record ManualRemoveOperationDetailData(
    bool IsFromWeb,
    string Language,
    bool IsForceRemove,
    List<string> ReturnedRoles,
    List<ChannelDetailData> ReturnedChannels
);
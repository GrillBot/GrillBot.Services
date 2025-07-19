namespace UnverifyService.Models.Response.Logs.Detail;

public record RecoveryOperationDetailData(
    List<string> ReturnedRoles,
    List<ChannelDetailData> ReturnedChannels
);
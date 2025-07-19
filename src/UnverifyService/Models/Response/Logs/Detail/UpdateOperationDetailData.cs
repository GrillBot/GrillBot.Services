namespace UnverifyService.Models.Response.Logs.Detail;

public record UpdateOperationDetailData(
    DateTime NewStartAtUtc,
    DateTime NewEndAtUtc,
    long NewDurationMilliseconds,
    DateTime OldStartAtUtc,
    DateTime OldEndAtUtc,
    long OldDurationMilliseconds,
    string Reason
);
using GrillBot.Core.Infrastructure.Actions;
using RubbergodService.MemberSynchronization;

namespace RubbergodService.Actions;

public class RefreshUserAction : ApiActionBase
{
    private MemberSyncQueue MemberSyncQueue { get; }

    public RefreshUserAction(MemberSyncQueue memberSyncQueue)
    {
        MemberSyncQueue = memberSyncQueue;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var memberId = (string)Parameters[0]!;

        await MemberSyncQueue.AddToQueueAsync(memberId);
        return new ApiResult(StatusCodes.Status200OK);
    }
}

using GrillBot.Core.Infrastructure.Actions;
using RubbergodService.Core.Entity;
using RubbergodService.Core.Repository;
using RubbergodService.MemberSynchronization;

namespace RubbergodService.Actions;

public class StoreKarmaAction : ApiActionBase
{
    private MemberSyncQueue MemberSyncQueue { get; }
    private RubbergodServiceRepository Repository { get; }

    public StoreKarmaAction(MemberSyncQueue queue, RubbergodServiceRepository repository)
    {
        MemberSyncQueue = queue;
        Repository = repository;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var items = (List<Karma>)Parameters[0]!;

        foreach (var item in items)
        {
            var karma = await Repository.Karma.FindKarmaByMemberIdAsync(item.MemberId);
            if (karma is null)
                await Repository.AddAsync(item);
            else
            {
                if (karma.IsEqual(item)) continue;
                karma.Update(item);
            }

            await MemberSyncQueue.AddToQueueAsync(item.MemberId);
        }

        await Repository.CommitAsync();
        return ApiResult.FromSuccess();
    }
}

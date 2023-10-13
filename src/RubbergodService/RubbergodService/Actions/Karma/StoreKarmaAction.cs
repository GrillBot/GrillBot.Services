using GrillBot.Core.Infrastructure.Actions;
using RubbergodService.Core.Entity;
using RubbergodService.Core.Repository;

namespace RubbergodService.Actions.Karma;

public class StoreKarmaAction : ApiActionBase
{
    private RubbergodServiceRepository Repository { get; }

    public StoreKarmaAction(RubbergodServiceRepository repository)
    {
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
                if (!karma.IsEqual(item))
                    karma.Update(item);
            }
        }

        await Repository.CommitAsync();
        return ApiResult.FromSuccess();
    }
}

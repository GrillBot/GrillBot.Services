using GrillBot.Core.Infrastructure.Actions;
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
        var items = (List<Core.Entity.Karma>)Parameters[0]!;

        foreach (var item in items.DistinctBy(o => o.MemberId))
        {
            var entity = await Repository.Karma.FindKarmaByMemberIdAsync(item.MemberId);
            if (entity is null)
                await Repository.AddAsync(item);
            else
                entity.Update(item);
        }

        await Repository.CommitAsync();
        return ApiResult.Ok();
    }
}

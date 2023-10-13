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
        var karmaItems = await Repository.Karma.GetItemsByMemberIdsAsync(items.Select(o => o.MemberId));
        var karmaItemsDict = karmaItems.ToDictionary(o => o.MemberId, o => o);

        foreach (var item in items)
        {
            if (karmaItemsDict.TryGetValue(item.MemberId, out var entity))
                entity.Update(item);
            else
                await Repository.AddAsync(item);
        }

        await Repository.CommitAsync();
        return ApiResult.FromSuccess();
    }
}

using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Models.Measures;

namespace UserMeasuresService.Actions.Measures;

public class DeleteMeasureAction : ApiAction<UserMeasuresContext>
{
    public DeleteMeasureAction(ICounterManager counterManager, UserMeasuresContext dbContext) : base(counterManager, dbContext)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<DeleteMeasuresRequest>(0);
        var itemFound = false;

        if (request.Id.HasValue)
        {
            itemFound |= await DeleteEntityAsync<MemberWarningItem>(request.Id.Value);
            itemFound |= await DeleteEntityAsync<UnverifyItem>(request.Id.Value);
            itemFound |= await DeleteEntityAsync<TimeoutItem>(request.Id.Value);
        }

        if (request.ExternalId.HasValue)
        {
            itemFound |= await DeleteEntityAsync(DbContext.Unverifies.Where(o => o.LogSetId == request.ExternalId.Value));
            itemFound |= await DeleteEntityAsync(DbContext.Timeouts.Where(o => o.ExternalId == request.ExternalId.Value));
        }

        return itemFound ? ApiResult.Ok() : ApiResult.NotFound();
    }

    private Task<bool> DeleteEntityAsync<TEntity>(Guid id) where TEntity : UserMeasureBase
    {
        var query = DbContext.Set<TEntity>().Where(o => o.Id == id);
        return DeleteEntityAsync(query);
    }

    private async Task<bool> DeleteEntityAsync<TEntity>(IQueryable<TEntity> query) where TEntity : UserMeasureBase
    {
        var item = await ContextHelper.ReadFirstOrDefaultEntityAsync(query);
        if (item is null)
            return false;

        DbContext.Remove(item);
        await ContextHelper.SaveChagesAsync();
        return true;
    }
}

using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models.Pagination;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Models.Measures;

namespace UserMeasuresService.Actions.Measures;

public class GetMeasuresList : ApiAction<UserMeasuresContext>
{
    public GetMeasuresList(UserMeasuresContext dbContext, ICounterManager counterManager) : base(counterManager, dbContext)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var parameters = (MeasuresListParams)Parameters[0]!;
        var warnings = await ReadEntitiesAsync<MemberWarningItem>(parameters, "Warning");
        var unverifies = await ReadEntitiesAsync<UnverifyItem>(parameters, "Unverify");
        var timeouts = await ReadEntitiesAsync<TimeoutItem>(parameters, "Timeout");
        var items = MapItems(warnings, unverifies, timeouts).OrderByDescending(o => o.CreatedAtUtc).ToList();
        var result = PaginatedResponse<MeasuresItem>.Create(items, parameters.Pagination);

        return ApiResult.Ok(result);
    }

    private async Task<List<TEntity>> ReadEntitiesAsync<TEntity>(MeasuresListParams parameters, string type) where TEntity : UserMeasureBase
    {
        if (!string.IsNullOrEmpty(parameters.Type) && parameters.Type != type)
            return new List<TEntity>();

        var query = DbContext.Set<TEntity>().AsNoTracking();

        if (!string.IsNullOrEmpty(parameters.GuildId))
            query = query.Where(o => o.GuildId == parameters.GuildId);
        if (!string.IsNullOrEmpty(parameters.UserId))
            query = query.Where(o => o.UserId == parameters.UserId);
        if (!string.IsNullOrEmpty(parameters.ModeratorId))
            query = query.Where(o => o.ModeratorId == parameters.ModeratorId);
        if (parameters.CreatedFrom is not null)
            query = query.Where(o => o.CreatedAtUtc >= parameters.CreatedFrom.Value);
        if (parameters.CreatedTo is not null)
            query = query.Where(o => o.CreatedAtUtc <= parameters.CreatedTo.Value);

        query = query.OrderByDescending(o => o.CreatedAtUtc);
        return await ContextHelper.ReadEntitiesAsync(query);
    }

    private IEnumerable<MeasuresItem> MapItems(List<MemberWarningItem> warnings, List<UnverifyItem> unverifies, List<TimeoutItem> timeouts)
    {
        foreach (var warning in warnings.Select(TransformBase))
        {
            warning.Type = "Warning";
            yield return warning;
        }

        foreach (var unverify in unverifies)
        {
            var unverifyItem = TransformBase(unverify);

            unverifyItem.Type = "Unverify";
            unverifyItem.ValidTo = unverify.ValidTo;

            yield return unverifyItem;
        }

        foreach (var timeout in timeouts)
        {
            var timeoutItem = TransformBase(timeout);

            timeoutItem.Type = "Timeout";
            timeoutItem.ValidTo = timeout.ValidTo;

            yield return timeoutItem;
        }
    }

    private MeasuresItem TransformBase(UserMeasureBase baseEntity)
    {
        return new MeasuresItem
        {
            ModeratorId = baseEntity.ModeratorId,
            GuildId = baseEntity.GuildId,
            CreatedAtUtc = baseEntity.CreatedAtUtc,
            UserId = baseEntity.UserId,
            Reason = baseEntity.Reason,
            MeasureId = baseEntity.Id
        };
    }
}

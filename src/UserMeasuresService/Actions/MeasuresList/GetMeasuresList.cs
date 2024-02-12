using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models.Pagination;
using Microsoft.EntityFrameworkCore;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Models.MeasuresList;

namespace UserMeasuresService.Actions.MeasuresList;

public class GetMeasuresList : ApiActionBase
{
    private UserMeasuresContext DbContext { get; }
    private ICounterManager CounterManager { get; }

    public GetMeasuresList(UserMeasuresContext dbContext, ICounterManager counterManager)
    {
        DbContext = dbContext;
        CounterManager = counterManager;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var parameters = (MeasuresListParams)Parameters[0]!;
        var warnings = await ReadWarningsAsync(parameters);
        var unverifies = await ReadUnverifiesAsync(parameters);
        var items = MapItems(warnings, unverifies).OrderByDescending(o => o.CreatedAtUtc).ToList();
        var result = PaginatedResponse<MeasuresItem>.Create(items, parameters.Pagination);

        return ApiResult.Ok(result);
    }

    private async Task<List<MemberWarningItem>> ReadWarningsAsync(MeasuresListParams parameters)
    {
        if (!string.IsNullOrEmpty(parameters.Type) && parameters.Type != "Warning")
            return new List<MemberWarningItem>();

        var query = DbContext.MemberWarnings.AsNoTracking();

        if (!string.IsNullOrEmpty(parameters.GuildId))
            query = query.Where(o => o.GuildId == parameters.GuildId);
        if (!string.IsNullOrEmpty(parameters.UserId))
            query = query.Where(o => o.UserId == parameters.UserId);
        if (!string.IsNullOrEmpty(parameters.ModeratorId))
            query = query.Where(o => o.ModeratorId == parameters.ModeratorId);
        if (parameters.CreatedFrom is not null)
            query = query.Where(o => o.CreatedAtUtc >= parameters.CreatedFrom);
        if (parameters.CreatedTo is not null)
            query = query.Where(o => o.CreatedAtUtc <= parameters.CreatedTo);

        query = query.OrderByDescending(o => o.CreatedAtUtc);
        using (CounterManager.Create("Api.MeasuresList.GetMeasuresList.Database"))
            return await query.ToListAsync();
    }

    private async Task<List<UnverifyItem>> ReadUnverifiesAsync(MeasuresListParams parameters)
    {
        if (!string.IsNullOrEmpty(parameters.Type) && parameters.Type != "Unverify")
            return new List<UnverifyItem>();

        var query = DbContext.Unverifies.AsNoTracking();

        if (!string.IsNullOrEmpty(parameters.GuildId))
            query = query.Where(o => o.GuildId == parameters.GuildId);
        if (!string.IsNullOrEmpty(parameters.UserId))
            query = query.Where(o => o.UserId == parameters.UserId);
        if (!string.IsNullOrEmpty(parameters.ModeratorId))
            query = query.Where(o => o.ModeratorId == parameters.ModeratorId);
        if (parameters.CreatedFrom is not null)
            query = query.Where(o => o.CreatedAtUtc >= parameters.CreatedFrom);
        if (parameters.CreatedTo is not null)
            query = query.Where(o => o.CreatedAtUtc <= parameters.CreatedTo);

        query = query.OrderByDescending(o => o.CreatedAtUtc);
        using (CounterManager.Create("Api.MeasuresList.GetMeasuresList.Database"))
            return await query.ToListAsync();
    }

    private IEnumerable<MeasuresItem> MapItems(List<MemberWarningItem> warnings, List<UnverifyItem> unverifies)
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
    }

    private MeasuresItem TransformBase(UserMeasureBase baseEntity)
    {
        return new MeasuresItem
        {
            ModeratorId = baseEntity.ModeratorId,
            GuildId = baseEntity.GuildId,
            CreatedAtUtc = baseEntity.CreatedAtUtc,
            UserId = baseEntity.UserId
        };
    }
}

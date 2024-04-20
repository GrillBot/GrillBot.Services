using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Models.Dashboard;

namespace UserMeasuresService.Actions.Dashboard;

public class GetDashboardData : ApiAction<UserMeasuresContext>
{
    public GetDashboardData(UserMeasuresContext dbContext, ICounterManager counterManager) : base(counterManager, dbContext)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var unverifies = await ReadRowsAsync<UnverifyItem>("Unverify");
        var warnings = await ReadRowsAsync<MemberWarningItem>("Warning");
        var timeouts = await ReadRowsAsync<TimeoutItem>("Timeout");
        var items = unverifies.Concat(warnings).Concat(timeouts);

        var result = items
            .OrderByDescending(o => o.CreatedAtUtc)
            .Take(10)
            .Select(o => new DashboardRow
            {
                Type = o.Type,
                UserId = o.UserId
            })
            .ToList();

        return ApiResult.Ok(result);
    }

    private async Task<List<InternalDashboardRow>> ReadRowsAsync<TEntity>(string type) where TEntity : UserMeasureBase
    {
        var query = DbContext.Set<TEntity>()
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAtUtc)
            .Take(10)
            .Select(o => new InternalDashboardRow
            {
                CreatedAtUtc = o.CreatedAtUtc,
                UserId = o.UserId
            });

        var rows = await ContextHelper.ReadEntitiesAsync(query);
        foreach (var row in rows)
            row.Type = type;

        return rows;
    }
}

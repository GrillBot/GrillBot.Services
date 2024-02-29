using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Models.Dashboard;

namespace UserMeasuresService.Actions.Dashboard;

public class GetDashboardData : ApiAction
{
    private UserMeasuresContext DbContext { get; }

    public GetDashboardData(UserMeasuresContext dbContext, ICounterManager counterManager) : base(counterManager)
    {
        DbContext = dbContext;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var unverifies = await ReadUnverifiesAsync();
        var warnings = await ReadWarningsAsync();
        var data = unverifies.Concat(warnings).OrderByDescending(o => o.CreatedAtUtc).Take(10).ToList();

        var result = data.ConvertAll(o => new DashboardRow
        {
            Type = o.Type,
            UserId = o.UserId
        });

        return ApiResult.Ok(result);
    }

    private async Task<List<InternalDashboardRow>> ReadUnverifiesAsync()
    {
        var query = DbContext.Unverifies.Select(o => new InternalDashboardRow
        {
            Type = "Unverify",
            CreatedAtUtc = o.CreatedAtUtc,
            UserId = o.UserId
        });

        return await ReadDashboardRowsAsync(query);
    }

    private async Task<List<InternalDashboardRow>> ReadWarningsAsync()
    {
        var query = DbContext.MemberWarnings.Select(o => new InternalDashboardRow
        {
            UserId = o.UserId,
            CreatedAtUtc = o.CreatedAtUtc,
            Type = "Warning"
        });

        return await ReadDashboardRowsAsync(query);
    }

    private async Task<List<InternalDashboardRow>> ReadDashboardRowsAsync(IQueryable<InternalDashboardRow> query)
    {
        using (CreateCounter("Database"))
        {
            return await query
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedAtUtc)
                .Take(10)
                .ToListAsync();
        }
    }
}

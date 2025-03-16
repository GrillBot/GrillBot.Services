using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetAuditLogStatisticsAction : ApiAction<AuditLogServiceContext>
{
    public GetAuditLogStatisticsAction(AuditLogServiceContext auditLogServiceContext, ICounterManager counterManager) : base(counterManager, auditLogServiceContext)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var result = new AuditLogStatistics
        {
            ByDate = await GetStatisticsByDateAsync(),
            ByType = await GetStatisticsByTypeAsync(),
            FileExtensionStatistics = await GetFileExtensionStatisticsAsync()
        };

        return ApiResult.Ok(result);
    }

    private Task<Dictionary<string, long>> GetStatisticsByTypeAsync()
    {
        var query = DbContext.LogItems.AsNoTracking()
            .GroupBy(o => o.Type)
            .OrderBy(o => o.Key)
            .Select(o => new { o.Key, Count = o.LongCount() });

        return ContextHelper.ReadToDictionaryAsync(query, o => o.Key.ToString(), o => o.Count);
    }

    private Task<Dictionary<string, long>> GetStatisticsByDateAsync()
    {
        var query = DbContext.LogItems.AsNoTracking()
            .GroupBy(o => new { o.LogDate.Year, o.LogDate.Month })
            .OrderBy(o => o.Key.Year)
            .ThenBy(o => o.Key.Month)
            .Select(o => new
            {
                Key = o.Key.Year + "-" + o.Key.Month.ToString().PadLeft(2, '0'),
                Count = o.LongCount()
            });

        return ContextHelper.ReadToDictionaryAsync(query, o => o.Key, o => o.Count);
    }

    private async Task<List<FileExtensionStatistic>> GetFileExtensionStatisticsAsync()
    {
        var query = DbContext.Files.AsNoTracking()
            .GroupBy(o => (o.Extension ?? ".NoExtension").ToLower())
            .Select(o => new FileExtensionStatistic
            {
                Extension = o.Key,
                Count = o.Count(),
                Size = o.Sum(x => x.Size)
            })
            .OrderBy(o => o.Extension);

        return await ContextHelper.ReadEntitiesAsync(query);
    }
}

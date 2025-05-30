using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetAuditLogStatisticsAction(
    AuditLogServiceContext auditLogServiceContext,
    ICounterManager counterManager
) : ApiAction<AuditLogServiceContext>(counterManager, auditLogServiceContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var result = new AuditLogStatistics
        {
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

using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.BackgroundServices.Actions;

public class ComputeFileExtensionStatisticsAction : PostProcessActionBase
{
    public ComputeFileExtensionStatisticsAction(AuditLogServiceContext context, AuditLogStatisticsContext statisticsContext) : base(context, statisticsContext)
    {
    }

    public override bool CanProcess(LogItem logItem)
        => logItem.Files.Count > 0;

    public override async Task ProcessAsync(LogItem logItem)
    {
        foreach (var file in logItem.Files.GroupBy(o => o.Extension).Select(o => o.Key))
        {
            var extension = file ?? ".NoExtension";
            var stats = await GetOrCreateStatisticEntity<FileExtensionStatistic>(o => o.Extension == extension, extension);
            var baseQuery = Context.Files.AsNoTracking().Where(o => o.Extension == file);

            stats.Extension = extension;
            stats.Count = await baseQuery.LongCountAsync();
            stats.Size = await baseQuery.SumAsync(o => o.Size);
        }

        await StatisticsContext.SaveChangesAsync();
    }
}

﻿using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.BackgroundServices.Actions;

public class ComputeAvgTimesAction : PostProcessActionBase
{
    public ComputeAvgTimesAction(AuditLogServiceContext context, AuditLogStatisticsContext statisticsContext) : base(context, statisticsContext)
    {
    }

    public override bool CanProcess(LogItem logItem)
        => logItem.Type is LogType.Api or LogType.InteractionCommand or LogType.JobCompleted;

    public override async Task ProcessAsync(LogItem logItem)
    {
        var date = DateOnly.FromDateTime(logItem.CreatedAt);
        var stats = await GetOrCreateStatisticEntity<DailyAvgTimes>(o => o.Date == date, date);

        var startOfday = date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        var endOfDay = date.ToDateTime(new TimeOnly(23, 59, 59, 999), DateTimeKind.Utc);

        await ProcessApiTimesAsync("V1", logItem, stats, date);
        await ProcessApiTimesAsync("V2", logItem, stats, date);
        await ProcessInteractionsAsync(logItem, stats, startOfday, endOfDay);
        await ProcessJobsAsync(logItem, stats, startOfday, endOfDay);

        stats.Date = date;
        await StatisticsContext.SaveChangesAsync();
    }

    private async Task ProcessApiTimesAsync(string expectedApiGroup, LogItem logItem, DailyAvgTimes stats, DateOnly requestDate)
    {
        if (logItem.Type is not LogType.Api || logItem.ApiRequest!.ApiGroupName != expectedApiGroup) return;

        var avgTime = -1D;
        var query = Context.ApiRequests.AsNoTracking()
            .Where(o => !Context.LogItems.Any(x => x.IsDeleted && o.LogItemId == x.Id))
            .Where(o => o.ApiGroupName == expectedApiGroup && o.RequestDate == requestDate);
        if (await query.AnyAsync())
            avgTime = await query.AverageAsync(o => o.Duration);

        if (expectedApiGroup == "V2")
            stats.ExternalApi = avgTime;
        else
            stats.InternalApi = avgTime;
    }

    private async Task ProcessInteractionsAsync(LogItem logItem, DailyAvgTimes stats, DateTime startOfDay, DateTime endOfday)
    {
        if (logItem.Type is not LogType.InteractionCommand) return;

        var avgTime = -1D;
        var query = Context.InteractionCommands.AsNoTracking()
            .Where(o => !Context.LogItems.Any(x => x.IsDeleted && o.LogItemId == x.Id))
            .Where(o => o.EndAt >= startOfDay && o.EndAt < endOfday);
        if (await query.AnyAsync())
            avgTime = await query.AverageAsync(o => o.Duration);

        stats.Interactions = avgTime;
    }

    private async Task ProcessJobsAsync(LogItem logItem, DailyAvgTimes stats, DateTime startOfDay, DateTime endOfDay)
    {
        if (logItem.Type is not LogType.JobCompleted) return;

        var avgTime = -1D;
        var query = Context.JobExecutions.AsNoTracking()
            .Where(o => !Context.LogItems.Any(x => x.IsDeleted && o.LogItemId == x.Id))
            .Where(o => o.EndAt >= startOfDay && o.EndAt < endOfDay);
        if (await query.AnyAsync())
            avgTime = await query.AverageAsync(o => o.Duration);

        stats.Jobs = avgTime;
    }
}

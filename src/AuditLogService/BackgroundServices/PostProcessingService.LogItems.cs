using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.BackgroundServices;

public partial class PostProcessingService
{
    private async Task<List<LogItem>> ReadItemsToProcessAsync(CancellationToken cancellationToken)
    {
        using (CounterManager.Create("BackgroundService.LogItems.Load"))
        {
            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AuditLogServiceContext>();

            var headers = await context.LogItems.AsNoTracking()
                .Where(o => o.IsPendingProcess)
                .OrderByDescending(o => o.CreatedAt)
                .Take(1000)
                .ToListAsync(cancellationToken);
            if (headers.Count == 0)
                return headers;

            foreach (var item in headers)
                await SetDataAsync(context, item, cancellationToken);

            var mergedItems = new List<LogItem>();
            foreach (var item in headers)
            {
                var mergedItem = FindRootItemForMerge(mergedItems, item);
                if (mergedItem is null)
                {
                    mergedItems.Add(item);
                    continue;
                }

                mergedItem.MergedItems.Add(item);
            }

            return mergedItems;
        }
    }

    private static async Task SetDataAsync(AuditLogServiceContext context, LogItem header, CancellationToken cancellationToken)
    {
        IQueryable<TData> WithCommonFilter<TData>(IQueryable<TData> query) where TData : ChildEntityBase
            => query.Where(o => o.LogItemId == header.Id).AsNoTracking();

        // For some types data is not loaded due to performance reasons.
        switch (header.Type)
        {
            case LogType.Info or LogType.Warning or LogType.Error:
            case LogType.ChannelCreated:
            case LogType.ChannelDeleted:
            case LogType.ChannelUpdated:
            case LogType.EmoteDeleted:
            case LogType.OverwriteCreated:
            case LogType.OverwriteDeleted:
            case LogType.OverwriteUpdated:
            case LogType.Unban:
            case LogType.MemberUpdated:
            case LogType.MemberRoleUpdated:
            case LogType.GuildUpdated:
            case LogType.UserLeft:
            case LogType.UserJoined:
            case LogType.MessageEdited:
                break;
            case LogType.MessageDeleted:
                var files = await context.Files.AsNoTracking().Where(o => o.LogItemId == header.Id).ToListAsync(cancellationToken);
                header.Files = files.ToHashSet();
                break;
            case LogType.InteractionCommand:
                header.InteractionCommand = await WithCommonFilter(context.InteractionCommands).FirstOrDefaultAsync(cancellationToken);
                break;
            case LogType.ThreadDeleted:
                break;
            case LogType.JobCompleted:
                header.Job = await WithCommonFilter(context.JobExecutions).FirstOrDefaultAsync(cancellationToken);
                break;
            case LogType.Api:
                header.ApiRequest = await WithCommonFilter(context.ApiRequests).FirstOrDefaultAsync(cancellationToken);
                break;
            case LogType.ThreadUpdated:
            case LogType.RoleDeleted:
                break;
        }
    }

    private static LogItem? FindRootItemForMerge(List<LogItem> mergedItems, LogItem item)
    {
        var query = mergedItems
            .Where(o => o.Type == item.Type && o.Id != item.Id);

        if (item.Type == LogType.InteractionCommand)
        {
            var interaction = item.InteractionCommand!;
            query = query.Where(o =>
                o.InteractionCommand is not null && interaction is not null &&
                o.InteractionCommand!.Name == interaction.Name &&
                o.InteractionCommand.ModuleName == interaction.ModuleName &&
                o.InteractionCommand.MethodName == interaction.MethodName &&
                o.InteractionCommand.IsSuccess == interaction.IsSuccess &&
                o.InteractionCommand.EndAt.Date == interaction.EndAt.Date &&
                o.UserId == item.UserId
            );
        }
        else if (item.Type == LogType.Api)
        {
            var request = item.ApiRequest!;
            query = query.Where(o =>
                o.ApiRequest is not null && request is not null &&
                o.ApiRequest!.RequestDate == request.RequestDate &&
                o.ApiRequest.Method == request.Method &&
                o.ApiRequest.TemplatePath == request.TemplatePath
            );
        }
        else if (item.Type == LogType.JobCompleted)
        {
            var job = item.Job!;
            query = query.Where(o =>
                o.Job is not null && job is not null &&
                o.Job!.EndAt.Date == job.EndAt.Date &&
                o.Job.JobName == job.JobName
            );
        }
        else if (item.Type == LogType.MessageDeleted)
        {
            var itemFileExtensions = item.Files.Select(o => o.Extension).Distinct().OrderBy(o => o).ToList();
            query = query.Where(o => o.Files.Select(x => x.Extension).Distinct().OrderBy(x => x).SequenceEqual(itemFileExtensions));
        }

        return query.FirstOrDefault();
    }

    private async Task ResetProcessFlagAsync(LogItem item)
    {
        using (CounterManager.Create("BackgroundService.LogItems.FlagReset"))
        {
            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AuditLogServiceContext>();

            await ResetProcessFlagAsync(context, new[] { item.Id });
            if (item.MergedItems.Count == 0)
            {
                await context.SaveChangesAsync();
                return;
            }

            var mergedItemIds = item.MergedItems.Select(o => o.Id).ToArray();
            await ResetProcessFlagAsync(context, mergedItemIds);
            await context.SaveChangesAsync();
        }
    }

    private static async Task ResetProcessFlagAsync(AuditLogServiceContext context, Guid[] ids)
    {
        var items = await context.LogItems.Where(o => ids.Contains(o.Id)).ToListAsync();
        foreach (var item in items)
            item.IsPendingProcess = false;
    }
}

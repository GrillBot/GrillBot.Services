using AuditLogService.Core.Entity;
using GrillBot.Services.Common.EntityFramework.Helpers;
using Microsoft.EntityFrameworkCore;

#pragma warning disable S4144 // Methods should not have identical implementations
namespace AuditLogService.Core.Extensions;

public static class LogItemExtensions
{
    public static async Task SetLogDataAsync<TData>(
        this IEnumerable<LogItem> headers,
        IQueryable<TData> query,
        Action<LogItem, TData> setData,
        ContextHelper<AuditLogServiceContext> contextHelper,
        bool disableTracking
    ) where TData : ChildEntityBase
    {
        var ids = headers.Select(o => o.Id).ToList();

        query = query.Where(o => ids.Contains(o.LogItemId));
        if (disableTracking)
            query = query.AsNoTracking();

        var data = await contextHelper.ReadEntitiesAsync(query);
        foreach (var item in data)
        {
            var header = headers.First(o => o.Id == item.LogItemId);
            setData(header, item);
        }
    }

    public static async Task SetLogDataWithoutKeyAsync<TData>(
        this IEnumerable<LogItem> headers,
        IQueryable<TData> query,
        Action<LogItem, TData> setData,
        ContextHelper<AuditLogServiceContext> contextHelper,
        bool disableTracking
    ) where TData : ChildEntityBaseWithoutKey
    {
        var ids = headers.Select(o => o.Id).ToList();

        query = query.Where(o => ids.Contains(o.LogItemId));
        if (disableTracking)
            query = query.AsNoTracking();

        var data = await contextHelper.ReadEntitiesAsync(query);
        foreach (var item in data)
        {
            var header = headers.First(o => o.Id == item.LogItemId);
            setData(header, item);
        }
    }
}

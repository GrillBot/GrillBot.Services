using System.Text.Json.Nodes;
using UnverifyService.Core.Entity.Logs;
using UnverifyService.Models.Response;

namespace UnverifyService.Actions.Archivation;

public partial class CreateArchivationDataAction
{
    private static ArchivationResult CreateArchive(List<UnverifyLogItem> items)
    {
        var result = new ArchivationResult();
        var jsonItems = new JsonArray();

        foreach (var item in items)
            jsonItems.Add(ProcessLogItem(item, result));

        var json = new JsonObject
        {
            ["CreatedAt"] = DateTime.UtcNow,
            ["Count"] = items.Count,
            ["Items"] = jsonItems
        };

        result.Content = json.ToJsonString(new() { WriteIndented = false });
        result.ItemsCount = jsonItems.Count;
        result.PerType = result.PerType.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);

        return result;
    }

    private static JsonObject ProcessLogItem(UnverifyLogItem item, ArchivationResult result)
    {
        result.Ids.Add(item.Id);

        var json = new JsonObject
        {
            ["Id"] = item.Id,
            ["Number"] = item.LogNumber,
            ["Type"] = item.OperationType.ToString(),
            ["CreatedAt"] = item.CreatedAt,
            ["GuildId"] = item.GuildId.Value,
            ["FromUserId"] = item.FromUserId.Value,
            ["ToUserId"] = item.ToUserId.Value
        };

        if (!result.GuildIds.Contains(item.GuildId))
            result.GuildIds.Add(item.GuildId);

        if (!result.UserIds.Contains(item.FromUserId))
            result.UserIds.Add(item.FromUserId);

        if (!result.UserIds.Contains(item.ToUserId))
            result.UserIds.Add(item.ToUserId);

        if (item.ParentLogItemId is not null)
            json["ParentId"] = item.ParentLogItemId;

        var dataJson = ProcessData(item, result);
        if (dataJson is not null)
            json["Data"] = dataJson;

        UpdateTypeStats(item, result);
        UpdateMonthStats(item, result);
        return json;
    }

    private static void UpdateTypeStats(UnverifyLogItem item, ArchivationResult result)
        => IncrementStats(item.OperationType.ToString(), result.PerType);

    private static void UpdateMonthStats(UnverifyLogItem item, ArchivationResult result)
        => IncrementStats(item.CreatedAt.ToString("MM-yyyy"), result.PerMonths);

    private static void IncrementStats(string key, Dictionary<string, int> stats)
    {
        stats.TryAdd(key, 0);
        stats[key]++;
    }
}

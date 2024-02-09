using System.Text.Json.Nodes;
using AuditLogService.Core.Entity;
using AuditLogService.Models.Response;
using File = AuditLogService.Core.Entity.File;

namespace AuditLogService.Actions.Archivation;

public partial class ArchiveOldLogsAction
{
    private static ArchivationResult CreateArchive(List<LogItem> items)
    {
        var result = new ArchivationResult();
        var jsonItems = new JsonArray();

        foreach (var item in items)
            jsonItems.Add(ProcessLogItem(item, result));

        var json = new JsonObject
        {
            ["CreatedAt"] = DateTime.UtcNow.ToString("o"),
            ["Count"] = items.Count,
            ["Items"] = jsonItems
        };

        result.Content = json.ToJsonString();
        result.ChannelIds = result.ChannelIds.Distinct().ToList();
        result.GuildIds = result.GuildIds.Distinct().ToList();
        result.UserIds = result.UserIds.Distinct().ToList();
        result.Files = result.Files.Distinct().ToList();
        result.ItemsCount = items.Count;
        result.TotalFilesSize = items.SelectMany(o => o.Files).Sum(o => o.Size);
        result.PerType = result.PerType.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);

        return result;
    }

    private static JsonNode ProcessLogItem(LogItem item, ArchivationResult result)
    {
        result.Ids.Add(item.Id);

        var json = new JsonObject
        {
            ["Id"] = item.Id.ToString(),
            ["CreatedAt"] = item.CreatedAt.ToString("o"),
            ["Type"] = item.Type.ToString()
        };

        if (!string.IsNullOrEmpty(item.GuildId))
        {
            result.GuildIds.Add(item.GuildId);
            json["GuildId"] = item.GuildId;
        }

        if (!string.IsNullOrEmpty(item.UserId))
        {
            result.UserIds.Add(item.UserId);
            json["UserId"] = item.UserId;
        }

        if (!string.IsNullOrEmpty(item.ChannelId))
        {
            result.ChannelIds.Add(item.ChannelId);
            json["ChannelId"] = item.ChannelId;
        }

        if (!string.IsNullOrEmpty(item.DiscordId))
            json["DiscordId"] = item.DiscordId;

        if (item.Files.Count > 0)
            json["Files"] = new JsonArray(item.Files.Select(o => ProcessFile(o, result)).ToArray());

        var dataJson = ProcessData(item, result);
        if (dataJson is not null)
            json["Data"] = dataJson;

        UpdateTypeStats(item, result);
        UpdateMonthStats(item, result);

        return json;
    }

    private static JsonNode ProcessFile(File file, ArchivationResult result)
    {
        result.Files.Add(file.Filename);

        var json = new JsonObject
        {
            ["Id"] = file.Id.ToString(),
            ["Filename"] = file.Filename,
            ["Size"] = file.Size
        };

        if (!string.IsNullOrEmpty(file.Extension))
            json["Extension"] = file.Extension;
        return json;
    }

    private static void UpdateTypeStats(LogItem item, ArchivationResult result)
        => IncrementStats(item.Type.ToString(), result.PerType);

    private static void UpdateMonthStats(LogItem item, ArchivationResult result)
        => IncrementStats(item.CreatedAt.ToString("MM-yyyy"), result.PerMonths);

    private static void IncrementStats(string key, Dictionary<string, long> stats)
    {
        if (!stats.ContainsKey(key))
            stats.Add(key, 0);
        stats[key]++;
    }
}

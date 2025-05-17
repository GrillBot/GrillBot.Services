using System.Text.Json.Nodes;
using AuditLogService.Core.Entity;
using AuditLogService.Models.Response;
using File = AuditLogService.Core.Entity.File;

namespace AuditLogService.Actions.Archivation;

public partial class CreateArchivationDataAction
{
    private static ArchivationResult CreateArchive(List<LogItem> items)
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

        result.Content = json.ToJsonString();
        result.ItemsCount = items.Count;
        result.PerType = result.PerType.OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.Value);

        return result;
    }

    private static JsonObject ProcessLogItem(LogItem item, ArchivationResult result)
    {
        result.Ids.Add(item.Id);

        var json = new JsonObject
        {
            ["Id"] = item.Id,
            ["CreatedAt"] = item.CreatedAt,
            ["Type"] = item.Type.ToString(),
            ["IsDeleted"] = item.IsDeleted
        };

        if (!string.IsNullOrEmpty(item.GuildId))
        {
            if (!result.GuildIds.Contains(item.GuildId))
                result.GuildIds.Add(item.GuildId);
            json["GuildId"] = item.GuildId;
        }

        if (!string.IsNullOrEmpty(item.UserId))
        {
            if (!result.UserIds.Contains(item.UserId))
                result.UserIds.Add(item.UserId);
            json["UserId"] = item.UserId;
        }

        if (!string.IsNullOrEmpty(item.ChannelId))
        {
            if (!result.ChannelIds.Contains(item.ChannelId))
                result.ChannelIds.Add(item.ChannelId);
            json["ChannelId"] = item.ChannelId;
        }

        if (!string.IsNullOrEmpty(item.DiscordId))
            json["DiscordId"] = item.DiscordId;

        if (item.Files.Count > 0)
            json["Files"] = new JsonArray([.. item.Files.Select(o => ProcessFile(o, result))]);

        var dataJson = ProcessData(item, result);
        if (dataJson is not null)
            json["Data"] = dataJson;

        UpdateTypeStats(item, result);
        UpdateMonthStats(item, result);

        return json;
    }

    private static JsonObject ProcessFile(File file, ArchivationResult result)
    {
        if (!result.Files.Contains(file.Filename))
            result.Files.Add(file.Filename);

        result.TotalFilesSize += file.Size;
        var json = new JsonObject
        {
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

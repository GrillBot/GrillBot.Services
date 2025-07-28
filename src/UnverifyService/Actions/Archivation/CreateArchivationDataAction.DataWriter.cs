using System.Text.Json.Nodes;
using UnverifyService.Core.Entity.Logs;
using UnverifyService.Core.Enums;
using UnverifyService.Models.Response;

namespace UnverifyService.Actions.Archivation;

public partial class CreateArchivationDataAction
{
    private static JsonObject? ProcessData(UnverifyLogItem item, ArchivationResult result)
    {
        return item.OperationType switch
        {
            UnverifyOperationType.Update => CreateUnverifyUpdateItem(item),
            UnverifyOperationType.SelfUnverify or UnverifyOperationType.Unverify => CreateUnverifySetItem(item, result),
            UnverifyOperationType.ManualRemove or UnverifyOperationType.AutoRemove or UnverifyOperationType.Recovery => CreateUnverifyRemoveItem(item, result),
            _ => null
        };
    }

    private static JsonObject? CreateUnverifyUpdateItem(UnverifyLogItem item)
    {
        if (item.UpdateOperation is null)
            return null;

        var json = new JsonObject
        {
            ["NewStartAt"] = item.UpdateOperation.NewStartAtUtc,
            ["NewEndAt"] = item.UpdateOperation.NewEndAtUtc
        };

        if (!string.IsNullOrEmpty(item.UpdateOperation.Reason))
            json["Reason"] = item.UpdateOperation.Reason;

        return json;
    }

    private static JsonObject? CreateUnverifySetItem(UnverifyLogItem item, ArchivationResult result)
    {
        if (item.SetOperation is null)
            return null;

        var json = new JsonObject
        {
            ["StartAt"] = item.SetOperation.StartAtUtc,
            ["EndAt"] = item.SetOperation.EndAtUtc,
            ["Language"] = item.SetOperation.Language,
            ["KeepMutedRole"] = item.SetOperation.KeepMutedRole
        };

        if (!string.IsNullOrEmpty(item.SetOperation.Reason))
            json["Reason"] = item.SetOperation.Reason;

        if (item.SetOperation.Roles.Count > 0)
        {
            json["Roles"] = new JsonObject(
                item.SetOperation.Roles.Select(o => new KeyValuePair<string, JsonNode?>(o.RoleId.ToString(), o.IsKept))
            );
        }

        if (item.SetOperation.Channels.Count > 0)
        {
            json["Channels"] = new JsonObject(
                item.SetOperation.Channels.Select(o => new KeyValuePair<string, JsonNode?>(
                    o.ChannelId.ToString(),
                    new JsonObject
                    {
                        ["AllowValue"] = o.AllowValue,
                        ["DenyValue"] = o.DenyValue,
                        ["IsKept"] = o.IsKept
                    }
                ))
            );

            result.ChannelIds.AddRange(
                item.SetOperation.Channels
                    .Select(o => o.ChannelId.Value)
                    .Where(id => !result.ChannelIds.Contains(id))
                    .Distinct()
            );
        }

        return json;
    }

    private static JsonObject? CreateUnverifyRemoveItem(UnverifyLogItem item, ArchivationResult result)
    {
        if (item.RemoveOperation is null)
            return null;

        var json = new JsonObject
        {
            ["IsFromWeb"] = item.RemoveOperation.IsFromWeb,
            ["Language"] = item.RemoveOperation.Language,
            ["Force"] = item.RemoveOperation.Force
        };

        if (item.RemoveOperation.Roles.Count > 0)
            json["Roles"] = new JsonArray([.. item.RemoveOperation.Roles.Select(o => o.RoleId.Value)]);

        if (item.RemoveOperation.Channels.Count > 0)
        {
            json["Channels"] = new JsonObject(
                item.RemoveOperation.Channels.Select(o => new KeyValuePair<string, JsonNode?>(
                    o.ChannelId.ToString(),
                    new JsonObject
                    {
                        ["AllowValue"] = o.AllowValue,
                        ["DenyValue"] = o.DenyValue
                    }
                ))
            );

            result.ChannelIds.AddRange(
                item.RemoveOperation.Channels
                    .Select(o => o.ChannelId.Value)
                    .Where(id => !result.ChannelIds.Contains(id))
                    .Distinct()
            );
        }

        return json;
    }
}

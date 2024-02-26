using System.Text.Json.Nodes;
using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Response;
using Discord;

namespace AuditLogService.Actions.Archivation;

public partial class CreateArchivationDataAction
{
    private static JsonNode? ProcessData(LogItem item, ArchivationResult result)
    {
        return item.Type switch
        {
            LogType.Info or LogType.Warning or LogType.Error => CreateLogMessageItem(item),
            LogType.ChannelCreated => CreateChannelCreatedItem(item),
            LogType.ChannelUpdated => CreateChannelUpdatedItem(item),
            LogType.ChannelDeleted => CreateChannelDeletedItem(item),
            LogType.EmoteDeleted => CreateDeletedEmoteItem(item),
            LogType.OverwriteCreated => CreateOverwriteCreatedItem(item, result),
            LogType.OverwriteDeleted => CreateOverwriteDeletedItem(item, result),
            LogType.OverwriteUpdated => CreateOverwriteUpdatedItem(item, result),
            LogType.Unban => CreateUnbanItem(item, result),
            LogType.MemberUpdated => CreateMemberUpdatedItem(item, result),
            LogType.MemberRoleUpdated => CreateMemberRoleUpdatedItems(item, result),
            LogType.GuildUpdated => CreateGuildUpdatedItems(item, result),
            LogType.UserLeft => CreateUserLeftItem(item, result),
            LogType.UserJoined => CreateUserJoinedItem(item),
            LogType.MessageEdited => CreateMessageEditedItem(item),
            LogType.MessageDeleted => CreateMessageDeletedItem(item, result),
            LogType.InteractionCommand => CreateInteractionItem(item),
            LogType.ThreadDeleted => CreateThreadDeletedItem(item),
            LogType.JobCompleted => CreateJobCompletedItem(item, result),
            LogType.Api => CreateApiRequestItem(item),
            LogType.ThreadUpdated => CreateThreadUpdatedItem(item),
            LogType.RoleDeleted => CreateRoleDeletedItem(item),
            _ => null
        };
    }

    private static JsonNode? CreateLogMessageItem(LogItem item)
    {
        if (item.LogMessage is null)
            return null;

        return new JsonObject
        {
            ["SourceAppName"] = item.LogMessage.SourceAppName,
            ["Source"] = item.LogMessage.Source,
            ["Severity"] = item.LogMessage.Severity.ToString(),
            ["Message"] = item.LogMessage.Message
        };
    }

    private static JsonNode? CreateChannelCreatedItem(LogItem item)
        => item.ChannelCreated is null ? null : (JsonNode)new JsonObject { ["ChannelInfo"] = CreateChannelInfoItems(item.ChannelCreated.ChannelInfo) };

    private static JsonNode? CreateChannelUpdatedItem(LogItem item)
    {
        if (item.ChannelUpdated is null)
            return null;

        return new JsonObject
        {
            ["Before"] = CreateChannelInfoItems(item.ChannelUpdated.Before),
            ["After"] = CreateChannelInfoItems(item.ChannelUpdated.After)
        };
    }

    private static JsonNode? CreateChannelDeletedItem(LogItem item)
        => item.ChannelDeleted is null ? null : new JsonObject { ["ChannelInfo"] = CreateChannelInfoItems(item.ChannelDeleted.ChannelInfo) };

    private static JsonNode CreateChannelInfoItems(ChannelInfo info)
    {
        var json = new JsonObject
        {
            ["InfoId"] = info.Id.ToString(),
            ["Position"] = info.Position,
            ["Flags"] = info.Flags
        };

        if (!string.IsNullOrEmpty(info.ChannelName))
            json["Name"] = info.ChannelName;

        if (info.SlowMode is not null)
            json["Slowmode"] = info.SlowMode.Value;

        if (info.ChannelType is not null)
            json["Type"] = info.ChannelType.Value.ToString();

        if (info.IsNsfw is not null)
            json["IsNsfw"] = info.IsNsfw.Value;

        if (info.Bitrate is not null)
            json["Bitrate"] = info.Bitrate.Value;

        return json;
    }

    private static JsonNode? CreateDeletedEmoteItem(LogItem item)
    {
        if (item.DeletedEmote is null)
            return null;

        return new JsonObject
        {
            ["Id"] = item.DeletedEmote.EmoteId,
            ["Name"] = item.DeletedEmote.EmoteName
        };
    }

    private static JsonNode? CreateOverwriteCreatedItem(LogItem item, ArchivationResult result)
        => item.OverwriteCreated is null ? null : new JsonObject { ["OverwriteInfo"] = CreateOverwriteInfoItems(item.OverwriteCreated.OverwriteInfo, result) };

    private static JsonNode? CreateOverwriteDeletedItem(LogItem item, ArchivationResult result)
        => item.OverwriteDeleted is null ? null : new JsonObject { ["OverwriteInfo"] = CreateOverwriteInfoItems(item.OverwriteDeleted.OverwriteInfo, result) };

    private static JsonNode? CreateOverwriteUpdatedItem(LogItem item, ArchivationResult result)
    {
        if (item.OverwriteUpdated is null)
            return null;

        return new JsonObject
        {
            ["Before"] = CreateOverwriteInfoItems(item.OverwriteUpdated.Before, result),
            ["After"] = CreateOverwriteInfoItems(item.OverwriteUpdated.After, result)
        };
    }

    private static JsonNode CreateOverwriteInfoItems(OverwriteInfo info, ArchivationResult result)
    {
        var json = new JsonObject
        {
            ["InfoId"] = info.Id.ToString(),
            ["TargetType"] = info.Target.ToString(),
            ["TargetId"] = info.TargetId
        };

        if (info.Target == PermissionTarget.User)
            result.UserIds.Add(info.TargetId);

        var perms = new OverwritePermissions(info.AllowValue, info.DenyValue);
        if (perms.AllowValue > 0)
            json["AllowList"] = new JsonArray(perms.ToAllowList().Select(o => JsonValue.Create(o.ToString())).ToArray());
        if (perms.DenyValue > 0)
            json["DenyList"] = new JsonArray(perms.ToDenyList().Select(o => JsonValue.Create(o.ToString())).ToArray());

        return json;
    }

    private static JsonNode? CreateUnbanItem(LogItem item, ArchivationResult result)
    {
        if (item.Unban is null)
            return null;

        result.UserIds.Add(item.Unban.UserId);
        return new JsonObject
        {
            ["UserId"] = item.Unban.UserId
        };
    }

    private static JsonNode? CreateMemberUpdatedItem(LogItem item, ArchivationResult result)
    {
        if (item.MemberUpdated is null)
            return null;

        return new JsonObject
        {
            ["Before"] = CreateMemberInfoItems(item.MemberUpdated.Before, result),
            ["After"] = CreateMemberInfoItems(item.MemberUpdated.After, result)
        };
    }

    private static JsonNode CreateMemberInfoItems(MemberInfo info, ArchivationResult result)
    {
        var json = new JsonObject
        {
            ["InfoId"] = info.Id.ToString(),
            ["UserId"] = info.UserId
        };

        result.UserIds.Add(info.UserId);
        if (!string.IsNullOrEmpty(info.Nickname))
            json["Nickname"] = info.Nickname;
        if (info.IsMuted is not null)
            json["IsMuted"] = info.IsMuted.Value;
        if (info.IsDeaf is not null)
            json["IsDeaf"] = info.IsDeaf.Value;
        if (!string.IsNullOrEmpty(info.SelfUnverifyMinimalTime))
            json["SelfUnverifyMinimalTime"] = info.SelfUnverifyMinimalTime;
        if (info.Flags is not null)
            json["Flags"] = info.Flags.Value;

        return json;
    }

    private static JsonNode? CreateMemberRoleUpdatedItems(LogItem item, ArchivationResult result)
    {
        if (item.MemberRolesUpdated is null || item.MemberRolesUpdated.Count == 0)
            return null;

        var roles = new JsonArray();
        foreach (var role in item.MemberRolesUpdated)
        {
            result.UserIds.Add(role.UserId);

            roles.Add(new JsonObject
            {
                ["InfoId"] = role.Id.ToString(),
                ["UserId"] = role.UserId,
                ["RoleId"] = role.RoleId,
                ["RoleName"] = role.RoleName,
                ["RoleColor"] = role.RoleColor,
                ["IsAdded"] = role.IsAdded
            });
        }

        return new JsonObject
        {
            ["Roles"] = roles
        };
    }

    private static JsonNode? CreateGuildUpdatedItems(LogItem item, ArchivationResult result)
    {
        if (item.GuildUpdated is null)
            return null;

        return new JsonObject
        {
            ["Before"] = CreateGuildUpdatedItems(item.GuildUpdated.Before, result),
            ["After"] = CreateGuildUpdatedItems(item.GuildUpdated.Before, result)
        };
    }

    private static JsonNode CreateGuildUpdatedItems(GuildInfo info, ArchivationResult result)
    {
        var json = new JsonObject
        {
            ["InfoId"] = info.Id.ToString(),
            ["DefaultMessageNotifications"] = info.DefaultMessageNotifications.ToString(),
            ["AfkTimeout"] = info.AfkTimeout,
            ["Name"] = info.Name,
            ["MfaLevel"] = info.MfaLevel.ToString(),
            ["VerificationLevel"] = info.VerificationLevel.ToString(),
            ["ExplicitContentFilter"] = info.ExplicitContentFilter.ToString(),
            ["PremiumTier"] = info.PremiumTier.ToString(),
            ["SystemChannelFlags"] = info.SystemChannelFlags.ToString(),
            ["NsfwLevel"] = info.NsfwLevel.ToString()
        };

        if (!string.IsNullOrEmpty(info.Description))
            json["Description"] = info.Description;
        if (!string.IsNullOrEmpty(info.VanityUrl))
            json["VanityUrl"] = info.VanityUrl;
        if (!string.IsNullOrEmpty(info.BannerId))
            json["BannerId"] = info.BannerId;
        if (!string.IsNullOrEmpty(info.DiscoverySplashId))
            json["DiscoverySplashId"] = info.DiscoverySplashId;
        if (!string.IsNullOrEmpty(info.SplashId))
            json["SplashId"] = info.SplashId;
        if (!string.IsNullOrEmpty(info.IconId))
            json["IconId"] = info.IconId;
        if (info.IconData is not null)
            json["IconData"] = Convert.ToBase64String(info.IconData);

        if (!string.IsNullOrEmpty(info.PublicUpdatesChannelId))
        {
            result.ChannelIds.Add(info.PublicUpdatesChannelId);
            json["PublicUpdatesChannelId"] = info.PublicUpdatesChannelId;
        }

        if (!string.IsNullOrEmpty(info.RulesChannelId))
        {
            result.ChannelIds.Add(info.RulesChannelId);
            json["RulesChannelId"] = info.RulesChannelId;
        }

        if (!string.IsNullOrEmpty(info.SystemChannelId))
        {
            result.ChannelIds.Add(info.SystemChannelId);
            json["SystemChannelId"] = info.SystemChannelId;
        }

        if (!string.IsNullOrEmpty(info.AfkChannelId))
        {
            result.ChannelIds.Add(info.AfkChannelId);
            json["AfkChannelId"] = info.AfkChannelId;
        }

        var features = Enum.GetValues<GuildFeature>()
            .Where(f => (info.Features & f) != GuildFeature.None)
            .Select(value => value.ToString())
            .ToList();
        if (features.Count > 0)
            json["Features"] = string.Join(", ", features);

        return json;
    }

    private static JsonNode? CreateUserLeftItem(LogItem item, ArchivationResult result)
    {
        if (item.UserLeft is null)
            return null;

        result.UserIds.Add(item.UserLeft.UserId);
        var json = new JsonObject
        {
            ["UserId"] = item.UserLeft.UserId,
            ["MemberCount"] = item.UserLeft.MemberCount,
            ["IsBan"] = item.UserLeft.IsBan.ToString()
        };

        if (!string.IsNullOrEmpty(item.UserLeft.BanReason))
            json["BanReason"] = item.UserLeft.BanReason;
        return json;
    }

    private static JsonNode? CreateUserJoinedItem(LogItem item)
    {
        if (item.UserJoined is null)
            return null;

        return new JsonObject
        {
            ["MemberCount"] = item.UserJoined.MemberCount
        };
    }

    private static JsonNode? CreateMessageEditedItem(LogItem item)
    {
        if (item.MessageEdited is null)
            return null;

        return new JsonObject
        {
            ["JumpUrl"] = item.MessageEdited.JumpUrl,
            ["ContentBefore"] = item.MessageEdited.ContentBefore,
            ["ContentAfter"] = item.MessageEdited.ContentAfter
        };
    }

    private static JsonNode? CreateMessageDeletedItem(LogItem item, ArchivationResult result)
    {
        if (item.MessageDeleted is null)
            return null;

        result.UserIds.Add(item.MessageDeleted.AuthorId);
        var json = new JsonObject
        {
            ["AuthorId"] = item.MessageDeleted.AuthorId,
            ["MessageCreatedAt"] = item.MessageDeleted.MessageCreatedAt.ToString("o")
        };

        if (!string.IsNullOrEmpty(item.MessageDeleted.Content))
            json["Content"] = item.MessageDeleted.Content;

        var embeds = new JsonArray();
        foreach (var embed in item.MessageDeleted.Embeds)
        {
            var embedJson = new JsonObject
            {
                ["EmbedId"] = embed.Id.ToString(),
                ["Type"] = embed.Type,
                ["ContainsFooter"] = embed.ContainsFooter.ToString()
            };

            if (!string.IsNullOrEmpty(embed.Title))
                embedJson["Title"] = embed.Title;
            if (!string.IsNullOrEmpty(embed.ImageInfo))
                embedJson["ImageInfo"] = embed.ImageInfo;
            if (!string.IsNullOrEmpty(embed.VideoInfo))
                embedJson["VideoInfo"] = embed.VideoInfo;
            if (!string.IsNullOrEmpty(embed.AuthorName))
                embedJson["AuthorName"] = embed.AuthorName;
            if (!string.IsNullOrEmpty(embed.ProviderName))
                embedJson["ProviderName"] = embed.ProviderName;
            if (!string.IsNullOrEmpty(embed.ThumbnailInfo))
                embedJson["ThumbnailInfo"] = embed.ThumbnailInfo;

            if (embed.Fields.Count > 0)
            {
                embedJson["Fields"] = new JsonArray(
                    embed.Fields.Select(f => new JsonObject
                    {
                        ["Name"] = f.Name,
                        ["Value"] = f.Value,
                        ["Inline"] = f.Inline
                    }).ToArray()
                );
            }

            embeds.Add(embedJson);
        }

        json["Embeds"] = embeds;
        return json;
    }

    private static JsonNode? CreateInteractionItem(LogItem item)
    {
        if (item.InteractionCommand is null)
            return null;

        var json = new JsonObject
        {
            ["Name"] = item.InteractionCommand.Name,
            ["ModuleName"] = item.InteractionCommand.ModuleName,
            ["MethodName"] = item.InteractionCommand.MethodName,
            ["HasResponded"] = item.InteractionCommand.HasResponded,
            ["IsValidToken"] = item.InteractionCommand.IsValidToken,
            ["IsSuccess"] = item.InteractionCommand.IsSuccess,
            ["Duration"] = item.InteractionCommand.Duration,
            ["Locale"] = item.InteractionCommand.Locale,
            ["EndAt"] = item.InteractionCommand.EndAt.ToString("o"),
            ["InteractionDate"] = item.InteractionCommand.InteractionDate.ToString("yyyy-MM-dd")
        };

        if (item.InteractionCommand.Parameters.Count > 0)
        {
            var parameters = new JsonArray();
            foreach (var parameter in item.InteractionCommand.Parameters)
            {
                var parametersJson = new JsonObject
                {
                    ["Name"] = parameter.Name,
                    ["Type"] = parameter.Type
                };

                if (!string.IsNullOrEmpty(parameter.Value))
                    parametersJson["Value"] = parameter.Value;

                parameters.Add(parametersJson);
            }

            json["Parameters"] = parameters;
        }

        if (item.InteractionCommand.CommandError is not null)
            json["CommandError"] = item.InteractionCommand.CommandError.Value;
        if (!string.IsNullOrEmpty(item.InteractionCommand.ErrorReason))
            json["ErrorReason"] = item.InteractionCommand.ErrorReason;
        if (!string.IsNullOrEmpty(item.InteractionCommand.Exception))
            json["Exception"] = item.InteractionCommand.Exception;
        return json;
    }

    private static JsonNode? CreateThreadDeletedItem(LogItem item)
    {
        if (item.ThreadDeleted is null)
            return null;

        return new JsonObject
        {
            ["ThreadInfo"] = CreateThreadInfoItems(item.ThreadDeleted.ThreadInfo)
        };
    }

    private static JsonNode CreateThreadInfoItems(ThreadInfo info)
    {
        var json = new JsonObject
        {
            ["InfoId"] = info.Id.ToString(),
            ["Name"] = info.ThreadName,
            ["Type"] = info.Type.ToString(),
            ["IsArchived"] = info.IsArchived.ToString(),
            ["ArchiveDuration"] = info.ArchiveDuration.ToString(),
            ["IsLocked"] = info.IsLocked.ToString()
        };

        if (info.SlowMode is not null)
            json["SlowMode"] = info.SlowMode.Value;

        if (info.Tags.Count > 0)
            json["Tags"] = new JsonArray(info.Tags.Select(t => JsonValue.Create(t)).ToArray());
        return json;
    }

    private static JsonNode? CreateJobCompletedItem(LogItem item, ArchivationResult result)
    {
        if (item.Job is null)
            return null;

        var json = new JsonObject
        {
            ["JobName"] = item.Job.JobName,
            ["Result"] = item.Job.Result,
            ["StartAt"] = item.Job.StartAt.ToString("o"),
            ["EndAt"] = item.Job.EndAt.ToString("o"),
            ["WasError"] = item.Job.WasError.ToString(),
            ["Duration"] = item.Job.Duration,
            ["JobDate"] = item.Job.JobDate.ToString("yyyy-MM-dd")
        };

        if (string.IsNullOrEmpty(item.Job.StartUserId))
            return json;

        result.UserIds.Add(item.Job.StartUserId);
        json["StartUserId"] = item.Job.StartUserId;
        return json;
    }

    private static JsonNode? CreateApiRequestItem(LogItem item)
    {
        if (item.ApiRequest is null)
            return null;

        var json = new JsonObject
        {
            ["ControllerName"] = item.ApiRequest.ControllerName,
            ["ActionName"] = item.ApiRequest.ActionName,
            ["StartAt"] = item.ApiRequest.StartAt.ToString("o"),
            ["EndAt"] = item.ApiRequest.EndAt.ToString("o"),
            ["Method"] = item.ApiRequest.Method,
            ["TemplatePath"] = item.ApiRequest.TemplatePath,
            ["Path"] = item.ApiRequest.Path,
            ["Language"] = item.ApiRequest.Language,
            ["ApiGroupName"] = item.ApiRequest.ApiGroupName,
            ["Identification"] = item.ApiRequest.Identification,
            ["Ip"] = item.ApiRequest.Ip,
            ["Result"] = item.ApiRequest.Result,
            ["IsSuccess"] = item.ApiRequest.IsSuccess,
            ["Duration"] = item.ApiRequest.Duration
        };

        if (item.ApiRequest.Parameters.Count > 0)
        {
            json["Parameters"] = new JsonArray(
                item.ApiRequest.Parameters.Select(p => new JsonObject
                {
                    ["Name"] = p.Key,
                    ["Value"] = p.Value
                }).ToArray()
            );
        }

        if (item.ApiRequest.Headers.Count > 0)
        {
            json["Headers"] = new JsonArray(
                item.ApiRequest.Headers.Select(h => new JsonObject
                {
                    ["Name"] = h.Key,
                    ["Value"] = h.Value
                }).ToArray()
            );
        }

        if (!string.IsNullOrEmpty(item.ApiRequest.Role))
            json["Role"] = item.ApiRequest.Role;

        if (!string.IsNullOrEmpty(item.ApiRequest.ForwardedIp))
            json["ForwardedIp"] = item.ApiRequest.ForwardedIp;

        return json;
    }

    private static JsonNode? CreateThreadUpdatedItem(LogItem item)
    {
        if (item.ThreadUpdated is null)
            return null;

        return new JsonObject
        {
            ["Before"] = CreateThreadInfoItems(item.ThreadUpdated.Before),
            ["After"] = CreateThreadInfoItems(item.ThreadUpdated.After)
        };
    }

    private static JsonNode? CreateRoleDeletedItem(LogItem item)
    {
        if (item.RoleDeleted?.RoleInfo is null)
            return null;

        return new JsonObject
        {
            ["Role"] = CreateRoleInfoItems(item.RoleDeleted.RoleInfo)
        };
    }

    private static JsonNode CreateRoleInfoItems(RoleInfo info)
    {
        var json = new JsonObject
        {
            ["InfoId"] = info.Id.ToString(),
            ["RoleId"] = info.RoleId,
            ["Name"] = info.Name,
            ["IsMentionable"] = info.IsMentionable.ToString(),
            ["IsHoisted"] = info.IsHoisted.ToString()
        };

        if (!string.IsNullOrEmpty(info.Color))
            json["Color"] = info.Color;

        if (info.Permissions.Count > 0)
        {
            json["Permissions"] = new JsonArray(
                info.Permissions.Select(p => JsonValue.Create(p)).ToArray()
            );
        }

        if (!string.IsNullOrEmpty(info.IconId))
            json["IconId"] = info.IconId;
        return json;
    }
}

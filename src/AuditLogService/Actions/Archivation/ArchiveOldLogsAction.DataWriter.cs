using System.Xml.Linq;
using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Response;
using Discord;

namespace AuditLogService.Actions.Archivation;

public partial class ArchiveOldLogsAction
{
    private static XElement? ProcessData(LogItem item, ArchivationResult result)
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

    private static XElement? CreateLogMessageItem(LogItem item)
    {
        if (item.LogMessage is null)
            return null;

        return new XElement(
            item.Type.ToString(),
            new XAttribute("SourceAppName", item.LogMessage.SourceAppName),
            new XAttribute("Source", item.LogMessage.Source),
            new XAttribute("Severity", item.LogMessage.Severity.ToString()),
            new XElement("Message", item.LogMessage.Message)
        );
    }

    private static XElement? CreateChannelCreatedItem(LogItem item)
        => item.ChannelCreated is null ? null : new XElement("ChannelCreated", CreateChannelInfoItems(item.ChannelCreated.ChannelInfo).ToArray());

    private static XElement? CreateChannelUpdatedItem(LogItem item)
    {
        if (item.ChannelUpdated is null)
            return null;

        return new XElement(
            "ChannelUpdated",
            new XElement("Before", CreateChannelInfoItems(item.ChannelUpdated.Before).ToArray()),
            new XElement("After", CreateChannelInfoItems(item.ChannelUpdated.After).ToArray())
        );
    }

    private static XElement? CreateChannelDeletedItem(LogItem item)
        => item.ChannelDeleted is null ? null : new XElement("ChannelDeleted", CreateChannelInfoItems(item.ChannelDeleted.ChannelInfo).ToArray());

    private static IEnumerable<object?> CreateChannelInfoItems(ChannelInfo info)
    {
        yield return new XAttribute("InfoId", info.Id.ToString());

        if (!string.IsNullOrEmpty(info.ChannelName))
            yield return new XAttribute("Name", info.ChannelName);

        if (info.SlowMode is not null)
            yield return new XAttribute("SlowMode", info.SlowMode.Value);

        if (info.ChannelType is not null)
            yield return new XAttribute("Type", info.ChannelType.Value);

        if (info.IsNsfw is not null)
            yield return new XAttribute("IsNsfw", info.IsNsfw.Value);

        if (info.Bitrate is not null)
            yield return new XAttribute("Bitrate", info.Bitrate.Value);

        yield return new XAttribute("Position", info.Position);
        yield return new XAttribute("Flags", info.Flags);
    }

    private static XElement? CreateDeletedEmoteItem(LogItem item)
    {
        if (item.DeletedEmote is null)
            return null;

        return new XElement(
            "EmoteDeleted",
            new XAttribute("Id", item.DeletedEmote.EmoteId),
            new XAttribute("Name", item.DeletedEmote.EmoteName)
        );
    }

    private static XElement? CreateOverwriteCreatedItem(LogItem item, ArchivationResult result)
        => item.OverwriteCreated is null ? null : new XElement("OverwriteCreated", CreateOverwriteInfoItems(item.OverwriteCreated.OverwriteInfo, result).ToArray());

    private static XElement? CreateOverwriteDeletedItem(LogItem item, ArchivationResult result)
        => item.OverwriteDeleted is null ? null : new XElement("OverwriteDeleted", CreateOverwriteInfoItems(item.OverwriteDeleted.OverwriteInfo, result).ToArray());

    private static XElement? CreateOverwriteUpdatedItem(LogItem item, ArchivationResult result)
    {
        if (item.OverwriteUpdated is null)
            return null;

        return new XElement(
            "OverwriteUpdated",
            new XElement("Before", CreateOverwriteInfoItems(item.OverwriteUpdated.Before, result).ToArray()),
            new XElement("After", CreateOverwriteInfoItems(item.OverwriteUpdated.After, result).ToArray())
        );
    }

    private static IEnumerable<object?> CreateOverwriteInfoItems(OverwriteInfo info, ArchivationResult result)
    {
        yield return new XAttribute("InfoId", info.Id.ToString());
        yield return new XAttribute("TargetType", info.Target.ToString());
        yield return new XAttribute("TargetId", info.TargetId);

        if (info.Target == PermissionTarget.User)
            result.UserIds.Add(info.TargetId);

        var perms = new OverwritePermissions(info.AllowValue, info.DenyValue);
        if (perms.AllowValue > 0)
            yield return new XAttribute("Allow", string.Join(", ", perms.ToAllowList()));
        if (perms.DenyValue > 0)
            yield return new XAttribute("Deny", string.Join(", ", perms.ToDenyList()));
    }

    private static XElement? CreateUnbanItem(LogItem item, ArchivationResult result)
    {
        if (item.Unban is null)
            return null;

        result.UserIds.Add(item.Unban.UserId);
        return new XElement(
            "Unban",
            new XAttribute("UserId", item.Unban.UserId)
        );
    }

    private static XElement? CreateMemberUpdatedItem(LogItem item, ArchivationResult result)
    {
        if (item.MemberUpdated is null)
            return null;

        return new XElement(
            "MemberUpdated",
            new XElement("Before", CreateMemberInfoItems(item.MemberUpdated.Before, result).ToArray()),
            new XElement("After", CreateMemberInfoItems(item.MemberUpdated.After, result).ToArray())
        );
    }

    private static IEnumerable<object> CreateMemberInfoItems(MemberInfo info, ArchivationResult result)
    {
        yield return new XAttribute("InfoId", info.Id.ToString());
        yield return new XAttribute("UserId", info.UserId);

        result.UserIds.Add(info.UserId);
        if (!string.IsNullOrEmpty(info.Nickname))
            yield return new XAttribute("Nickname", info.Nickname);
        if (info.IsMuted is not null)
            yield return new XAttribute("IsMuted", info.IsMuted.Value);
        if (info.IsDeaf is not null)
            yield return new XAttribute("IsDeaf", info.IsDeaf.Value);
        if (!string.IsNullOrEmpty(info.SelfUnverifyMinimalTime))
            yield return new XAttribute("SelfUnverifyMinimalTime", info.SelfUnverifyMinimalTime);
        if (info.Flags is not null)
            yield return new XAttribute("Flags", info.Flags.Value);
    }

    private static XElement? CreateMemberRoleUpdatedItems(LogItem item, ArchivationResult result)
    {
        if (item.MemberRolesUpdated is null || item.MemberRolesUpdated.Count == 0)
            return null;

        var xml = new XElement("MemberRoleUpdated");
        foreach (var role in item.MemberRolesUpdated)
        {
            result.UserIds.Add(role.UserId);

            xml.Add(new XElement(
                "Role",
                new XAttribute("InfoId", role.Id.ToString()),
                new XAttribute("UserId", role.UserId),
                new XAttribute("RoleId", role.RoleId),
                new XAttribute("RoleName", role.RoleName),
                new XAttribute("RoleColor", role.RoleColor),
                new XAttribute("IsAdded", role.IsAdded)
            ));
        }

        return xml;
    }

    private static XElement? CreateGuildUpdatedItems(LogItem item, ArchivationResult result)
    {
        if (item.GuildUpdated is null)
            return null;

        return new XElement(
            "GuildUpdated",
            new XElement("Before", CreateGuildUpdatedItems(item.GuildUpdated.Before, result)),
            new XElement("After", CreateGuildUpdatedItems(item.GuildUpdated.Before, result))
        );
    }

    private static IEnumerable<object> CreateGuildUpdatedItems(GuildInfo info, ArchivationResult result)
    {
        yield return new XAttribute("InfoId", info.Id.ToString());
        yield return new XAttribute("DefaultMessageNotifications", info.DefaultMessageNotifications.ToString());

        if (!string.IsNullOrEmpty(info.Description))
            yield return new XAttribute("Description", info.Description);
        if (!string.IsNullOrEmpty(info.VanityUrl))
            yield return new XAttribute("VanityUrl", info.VanityUrl);
        if (!string.IsNullOrEmpty(info.BannerId))
            yield return new XAttribute("BannerId", info.BannerId);
        if (!string.IsNullOrEmpty(info.DiscoverySplashId))
            yield return new XAttribute("DiscoverySplashId", info.DiscoverySplashId);
        if (!string.IsNullOrEmpty(info.SplashId))
            yield return new XAttribute("SplashId", info.SplashId);
        if (!string.IsNullOrEmpty(info.IconId))
            yield return new XAttribute("IconId", info.IconId);
        if (info.IconData is not null)
            yield return new XElement("IconData", Convert.ToBase64String(info.IconData));

        if (!string.IsNullOrEmpty(info.PublicUpdatesChannelId))
        {
            result.ChannelIds.Add(info.PublicUpdatesChannelId);
            yield return new XAttribute("PublicUpdatesChannelId", info.PublicUpdatesChannelId);
        }

        if (!string.IsNullOrEmpty(info.RulesChannelId))
        {
            result.ChannelIds.Add(info.RulesChannelId);
            yield return new XAttribute("RulesChannelId", info.RulesChannelId);
        }

        if (!string.IsNullOrEmpty(info.SystemChannelId))
        {
            result.ChannelIds.Add(info.SystemChannelId);
            yield return new XAttribute("SystemChannelId", info.SystemChannelId);
        }

        if (!string.IsNullOrEmpty(info.AfkChannelId))
        {
            result.ChannelIds.Add(info.AfkChannelId);
            yield return new XAttribute("AfkChannelId", info.AfkChannelId);
        }

        yield return new XAttribute("AfkTimeout", info.AfkTimeout);
        yield return new XAttribute("Name", info.Name);
        yield return new XAttribute("MfaLevel", info.MfaLevel.ToString());
        yield return new XAttribute("VerificationLevel", info.VerificationLevel.ToString());
        yield return new XAttribute("ExplicitContentFilter", info.ExplicitContentFilter.ToString());

        var features = Enum.GetValues<GuildFeature>()
            .Where(f => (info.Features & f) != GuildFeature.None)
            .Select(value => value.ToString())
            .ToList();
        if (features.Count > 0)
            yield return new XAttribute("Features", string.Join(", ", features));

        yield return new XAttribute("PremiumTier", info.PremiumTier.ToString());
        yield return new XAttribute("SystemChannelFlags", info.SystemChannelFlags.ToString());
        yield return new XAttribute("NsfwLevel", info.NsfwLevel.ToString());
    }

    private static XElement? CreateUserLeftItem(LogItem item, ArchivationResult result)
    {
        if (item.UserLeft is null)
            return null;

        result.UserIds.Add(item.UserLeft.UserId);
        var xml = new XElement(
            "UserLeft",
            new XAttribute("UserId", item.UserLeft.UserId),
            new XAttribute("MemberCount", item.UserLeft.MemberCount),
            new XAttribute("IsBan", item.UserLeft.IsBan.ToString())
        );

        if (!string.IsNullOrEmpty(item.UserLeft.BanReason))
            xml.Add(new XAttribute("BanReason", item.UserLeft.BanReason));
        return xml;
    }

    private static XElement? CreateUserJoinedItem(LogItem item)
    {
        if (item.UserJoined is null)
            return null;

        return new XElement(
            "UserJoined",
            new XAttribute("MemberCount", item.UserJoined.MemberCount)
        );
    }

    private static XElement? CreateMessageEditedItem(LogItem item)
    {
        if (item.MessageEdited is null)
            return null;

        return new XElement(
            "MessageEdited",
            new XAttribute("JumpUrl", item.MessageEdited.JumpUrl),
            new XElement("ContentBefore", item.MessageEdited.ContentBefore),
            new XElement("ContentAfter", item.MessageEdited.ContentAfter)
        );
    }

    private static XElement? CreateMessageDeletedItem(LogItem item, ArchivationResult result)
    {
        if (item.MessageDeleted is null)
            return null;

        result.UserIds.Add(item.MessageDeleted.AuthorId);
        var xml = new XElement(
            "MessageDeleted",
            new XAttribute("AuthorId", item.MessageDeleted.AuthorId),
            new XAttribute("MessageCreatedAt", item.MessageDeleted.MessageCreatedAt.ToString("o"))
        );

        if (!string.IsNullOrEmpty(item.MessageDeleted.Content))
            xml.Add(new XElement("Content", item.MessageDeleted.Content));

        foreach (var embed in item.MessageDeleted.Embeds)
        {
            var element = new XElement(
                "Embed",
                new XAttribute("EmbedId", embed.Id.ToString()),
                new XAttribute("Type", embed.Type),
                new XAttribute("ContainsFooter", embed.ContainsFooter.ToString())
            );

            if (!string.IsNullOrEmpty(embed.Title))
                element.Add(new XAttribute("Title", embed.Title));
            if (!string.IsNullOrEmpty(embed.ImageInfo))
                element.Add(new XAttribute("ImageInfo", embed.ImageInfo));
            if (!string.IsNullOrEmpty(embed.VideoInfo))
                element.Add(new XAttribute("VideoInfo", embed.VideoInfo));
            if (!string.IsNullOrEmpty(embed.AuthorName))
                element.Add(new XAttribute("AuthorName", embed.AuthorName));
            if (!string.IsNullOrEmpty(embed.ProviderName))
                element.Add(new XAttribute("ProviderName", embed.ProviderName));
            if (!string.IsNullOrEmpty(embed.ThumbnailInfo))
                element.Add(new XAttribute("ThumbnailInfo", embed.ThumbnailInfo));

            foreach (var field in embed.Fields)
            {
                element.Add(new XElement(
                    "Field",
                    new XAttribute("Name", field.Name),
                    new XAttribute("Value", field.Value),
                    new XAttribute("Inline", field.Inline)
                ));
            }

            xml.Add(element);
        }

        return xml;
    }

    private static XElement? CreateInteractionItem(LogItem item)
    {
        if (item.InteractionCommand is null)
            return null;

        var xml = new XElement(
            "Interaction",
            new XAttribute("Name", item.InteractionCommand.Name),
            new XAttribute("ModuleName", item.InteractionCommand.ModuleName),
            new XAttribute("MethodName", item.InteractionCommand.MethodName),
            new XAttribute("HasResponded", item.InteractionCommand.HasResponded),
            new XAttribute("IsValidToken", item.InteractionCommand.IsValidToken),
            new XAttribute("IsSuccess", item.InteractionCommand.IsSuccess),
            new XAttribute("Duration", item.InteractionCommand.Duration),
            new XAttribute("Locale", item.InteractionCommand.Locale)
        );

        foreach (var parameter in item.InteractionCommand.Parameters)
        {
            var element = new XElement(
                "Parameter",
                new XAttribute("Name", parameter.Name),
                new XAttribute("Type", parameter.Type)
            );

            if (!string.IsNullOrEmpty(parameter.Value))
                element.Add(new XAttribute("Value", parameter.Value));

            xml.Add(element);
        }

        if (item.InteractionCommand.CommandError is not null)
            xml.Add(new XAttribute("CommandError", item.InteractionCommand.CommandError.Value));
        if (!string.IsNullOrEmpty(item.InteractionCommand.ErrorReason))
            xml.Add(new XAttribute("ErrorReason", item.InteractionCommand.ErrorReason));
        if (!string.IsNullOrEmpty(item.InteractionCommand.Exception))
            xml.Add(new XAttribute("Exception", item.InteractionCommand.Exception));
        return xml;
    }

    private static XElement? CreateThreadDeletedItem(LogItem item)
    {
        if (item.ThreadDeleted is null)
            return null;

        return new XElement(
            "ThreadDeleted",
            CreateThreadInfoItems(item.ThreadDeleted.ThreadInfo).ToArray()
        );
    }

    private static IEnumerable<object> CreateThreadInfoItems(ThreadInfo info)
    {
        yield return new XAttribute("InfoId", info.Id.ToString());
        yield return new XAttribute("Name", info.ThreadName);

        if (info.SlowMode is not null)
            yield return new XAttribute("SlowMode", info.SlowMode.Value);

        yield return new XAttribute("Type", info.Type.ToString());
        yield return new XAttribute("IsArchived", info.IsArchived.ToString());
        yield return new XAttribute("ArchiveDuration", info.ArchiveDuration.ToString());
        yield return new XAttribute("IsLocked", info.IsLocked.ToString());

        if (info.Tags.Count > 0)
            yield return new XAttribute("Tags", string.Join(", ", info.Tags));
    }

    private static XElement? CreateJobCompletedItem(LogItem item, ArchivationResult result)
    {
        if (item.Job is null)
            return null;

        var xml = new XElement(
            "JobCompleted",
            new XAttribute("JobName", item.Job.JobName),
            new XElement("Result", item.Job.Result),
            new XAttribute("StartAt", item.Job.StartAt.ToString("o")),
            new XAttribute("EndAt", item.Job.EndAt.ToString("o")),
            new XAttribute("WasError", item.Job.WasError.ToString())
        );

        if (string.IsNullOrEmpty(item.Job.StartUserId))
            return xml;

        result.UserIds.Add(item.Job.StartUserId);
        xml.Add(new XAttribute("StartUserId", item.Job.StartUserId));
        return xml;
    }

    private static XElement? CreateApiRequestItem(LogItem item)
    {
        if (item.ApiRequest is null)
            return null;

        var xml = new XElement(
            "Api",
            new XAttribute("ControllerName", item.ApiRequest.ControllerName),
            new XAttribute("ActionName", item.ApiRequest.ActionName),
            new XAttribute("StartAt", item.ApiRequest.StartAt.ToString("o")),
            new XAttribute("EndAt", item.ApiRequest.EndAt.ToString("o")),
            new XAttribute("Method", item.ApiRequest.Method),
            new XAttribute("TemplatePath", item.ApiRequest.TemplatePath),
            new XAttribute("Path", item.ApiRequest.Path),
            new XAttribute("Language", item.ApiRequest.Language),
            new XAttribute("ApiGroupName", item.ApiRequest.ApiGroupName),
            new XAttribute("Identification", item.ApiRequest.Identification),
            new XAttribute("Ip", item.ApiRequest.Ip),
            new XAttribute("Result", item.ApiRequest.Result),
            new XAttribute("IsSuccess", item.ApiRequest.IsSuccess)
        );

        foreach (var param in item.ApiRequest.Parameters)
        {
            xml.Add(new XElement(
                "Parameter",
                new XAttribute("Name", param.Key),
                new XAttribute("Value", param.Value)
            ));
        }

        foreach (var param in item.ApiRequest.Headers)
        {
            xml.Add(new XElement(
                "Header",
                new XAttribute("Name", param.Key),
                new XAttribute("Value", param.Value)
            ));
        }

        if (!string.IsNullOrEmpty(item.ApiRequest.Role))
            xml.Add(new XAttribute("Role", item.ApiRequest.Role));

        if (!string.IsNullOrEmpty(item.ApiRequest.ForwardedIp))
            xml.Add(new XAttribute("ForwardedIp", item.ApiRequest.ForwardedIp));

        return xml;
    }

    private static XElement? CreateThreadUpdatedItem(LogItem item)
    {
        if (item.ThreadUpdated is null)
            return null;

        return new XElement(
            "ThreadUpdated",
            new XElement("Before", CreateThreadInfoItems(item.ThreadUpdated.Before).ToArray()),
            new XElement("After", CreateThreadInfoItems(item.ThreadUpdated.After).ToArray())
        );
    }

    private static XElement? CreateRoleDeletedItem(LogItem item)
    {
        if (item.RoleDeleted?.RoleInfo is null)
            return null;

        return new XElement(
            "RoleDeleted",
            new XElement("Role", CreateRoleInfoItems(item.RoleDeleted.RoleInfo).ToArray())
        );
    }

    private static IEnumerable<object> CreateRoleInfoItems(RoleInfo info)
    {
        yield return new XAttribute("InfoId", info.Id);
        yield return new XAttribute("RoleId", info.RoleId);
        yield return new XAttribute("Name", info.Name);

        if (!string.IsNullOrEmpty(info.Color))
            yield return new XAttribute("Color", info.Color);

        yield return new XAttribute("IsMentionable", info.IsMentionable.ToString());
        yield return new XAttribute("IsHoisted", info.IsHoisted.ToString());

        if (info.Permissions.Count > 0)
        {
            yield return new XElement(
                "Permissions",
                new XAttribute("Value", string.Join(", ", info.Permissions))
            );
        }

        if (!string.IsNullOrEmpty(info.IconId))
            yield return new XAttribute("IconId", info.IconId);
    }
}

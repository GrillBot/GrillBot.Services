using AuditLogService.Core.Entity;
using AuditLogService.Core.Extensions;
using AuditLogService.Core.Helpers;
using AuditLogService.Models.Response.Detail;
using Discord;
using GrillBot.Core.Models;
using Microsoft.EntityFrameworkCore;
using EmbedField = AuditLogService.Models.Response.Detail.EmbedField;
using InteractionCommandParameter = AuditLogService.Models.Response.Detail.InteractionCommandParameter;

namespace AuditLogService.Actions.Detail;

public partial class ReadDetailAction
{
    private IQueryable<TData> GetBaseQuery<TData>(LogItem header) where TData : ChildEntityBase
        => Context.Set<TData>().Where(o => o.LogItemId == header.Id).AsNoTracking();

    private async Task<MessageDetail?> CreateMessageDetailAsync(LogItem header)
    {
        return await GetBaseQuery<Core.Entity.LogMessage>(header)
            .Select(o => new MessageDetail
            {
                Text = o.Message,
                Source = o.Source,
                SourceAppName = o.SourceAppName
            })
            .FirstOrDefaultAsync();
    }

    private async Task<ChannelUpdatedDetail?> CreateChannelUpdatedDetailAsync(LogItem header)
    {
        var channelInfos = await GetBaseQuery<ChannelUpdated>(header)
            .Select(o => new { o.Before, o.After })
            .FirstOrDefaultAsync();
        if (channelInfos is null)
            return null;

        var before = channelInfos.Before;
        var after = channelInfos.After;

        var result = new ChannelUpdatedDetail
        {
            Bitrate = new Diff<int?>(before.Bitrate, after.Bitrate).NullIfEquals(),
            Flags = new Diff<int>(before.Flags, after.Flags).NullIfEquals(),
            Position = new Diff<int>(before.Position, after.Position).NullIfEquals(),
            SlowMode = new Diff<int?>(before.SlowMode, after.SlowMode).NullIfEquals(),
            IsNsfw = new Diff<bool?>(before.IsNsfw, after.IsNsfw).NullIfEquals(),
            Topic = new Diff<string?>(before.Topic, after.Topic).NullIfEquals(),
            Name = new Diff<string?>(before.ChannelName, after.ChannelName).NullIfEquals()
        };

        return ModelHelper.IsModelEmpty(result) ? null : result;
    }

    private async Task<OverwriteUpdatedDetail?> CreateOverwriteUpdatedDetailAsync(LogItem header)
    {
        var overwriteInfos = await GetBaseQuery<OverwriteUpdated>(header)
            .Select(o => new { o.Before, o.After })
            .FirstOrDefaultAsync();
        if (overwriteInfos is null)
            return null;

        var before = overwriteInfos.Before;
        var after = overwriteInfos.After;

        var permsBefore = new OverwritePermissions(before.AllowValue, before.DenyValue);
        var permsAfter = new OverwritePermissions(after.AllowValue, after.DenyValue);

        return new OverwriteUpdatedDetail
        {
            Allow = new Diff<List<string>>(permsBefore.ToAllowList().ConvertAll(o => o.ToString()), permsAfter.ToAllowList().ConvertAll(o => o.ToString())),
            Deny = new Diff<List<string>>(permsBefore.ToDenyList().ConvertAll(o => o.ToString()), permsAfter.ToDenyList().ConvertAll(o => o.ToString())),
            TargetId = before.TargetId,
            TargetType = before.Target
        };
    }

    private async Task<MemberUpdatedDetail?> CreateMemberUpdatedDetailAsync(LogItem header)
    {
        var memberInfos = await GetBaseQuery<MemberUpdated>(header)
            .Select(o => new { o.Before, o.After })
            .FirstOrDefaultAsync();
        if (memberInfos is null)
            return null;

        var before = memberInfos.Before;
        var after = memberInfos.After;

        var result = new MemberUpdatedDetail
        {
            IsMuted = new Diff<bool?>(before.IsMuted, after.IsMuted).NullIfEquals(),
            Flags = new Diff<int?>(before.Flags, after.Flags).NullIfEquals(),
            SelfUnverifyMinimalTime = new Diff<string?>(before.SelfUnverifyMinimalTime, after.SelfUnverifyMinimalTime).NullIfEquals(),
            Nickname = new Diff<string?>(before.Nickname, after.Nickname).NullIfEquals(),
            IsDeaf = new Diff<bool?>(before.IsDeaf, after.IsDeaf).NullIfEquals(),
            UserId = before.UserId
        };

        return ModelHelper.IsModelEmpty(result) ? null : result;
    }

    private async Task<GuildUpdatedDetail?> CreateGuildUpdatedDetailAsync(LogItem header)
    {
        var guildInfos = await GetBaseQuery<GuildUpdated>(header)
            .Select(o => new { o.Before, o.After })
            .FirstOrDefaultAsync();
        if (guildInfos is null)
            return null;

        var before = guildInfos.Before;
        var after = guildInfos.After;

        var result = new GuildUpdatedDetail
        {
            Name = new Diff<string>(before.Name, after.Name).NullIfEquals(),
            Description = new Diff<string?>(before.Description, after.Description).NullIfEquals(),
            Features = new Diff<GuildFeature>(before.Features, after.Features).NullIfEquals(),
            AfkTimeout = new Diff<int>(before.AfkTimeout, after.AfkTimeout).NullIfEquals(),
            BannerId = new Diff<string?>(before.BannerId, after.BannerId).NullIfEquals(),
            IconData = new Diff<byte[]?>(before.IconData, after.IconData).NullIfEquals(),
            IconId = new Diff<string?>(before.IconId, after.IconId).NullIfEquals(),
            MfaLevel = new Diff<MfaLevel>(before.MfaLevel, after.MfaLevel).NullIfEquals(),
            NsfwLevel = new Diff<NsfwLevel>(before.NsfwLevel, after.NsfwLevel).NullIfEquals(),
            PremiumTier = new Diff<PremiumTier>(before.PremiumTier, after.PremiumTier).NullIfEquals(),
            SplashId = new Diff<string?>(before.SplashId, after.SplashId).NullIfEquals(),
            VanityUrl = new Diff<string?>(before.VanityUrl, after.VanityUrl).NullIfEquals(),
            VerificationLevel = new Diff<VerificationLevel>(before.VerificationLevel, after.VerificationLevel).NullIfEquals(),
            AfkChannelId = new Diff<string?>(before.AfkChannelId, after.AfkChannelId).NullIfEquals(),
            DefaultMessageNotifications = new Diff<DefaultMessageNotifications>(before.DefaultMessageNotifications, after.DefaultMessageNotifications).NullIfEquals(),
            DiscoverySplashId = new Diff<string?>(before.DiscoverySplashId, after.DiscoverySplashId).NullIfEquals(),
            ExplicitContentFilter = new Diff<ExplicitContentFilterLevel>(before.ExplicitContentFilter, after.ExplicitContentFilter).NullIfEquals(),
            RulesChannelId = new Diff<string?>(before.RulesChannelId, after.RulesChannelId).NullIfEquals(),
            SystemChannelFlags = new Diff<SystemChannelMessageDeny>(before.SystemChannelFlags, after.SystemChannelFlags).NullIfEquals(),
            SystemChannelId = new Diff<string?>(before.SystemChannelId, after.SystemChannelId).NullIfEquals(),
            PublicUpdatesChannelId = new Diff<string?>(before.PublicUpdatesChannelId, after.PublicUpdatesChannelId).NullIfEquals()
        };

        return ModelHelper.IsModelEmpty(result) ? null : result;
    }

    private async Task<MessageDeletedDetail?> CreateMessageDeletedDetailAsync(LogItem header)
    {
        return await GetBaseQuery<MessageDeleted>(header)
            .Select(o => new MessageDeletedDetail
            {
                MessageCreatedAt = o.MessageCreatedAt,
                AuthorId = o.AuthorId,
                Content = o.Content,
                Embeds = o.Embeds.Select(e => new EmbedDetail
                {
                    Fields = e.Fields.Select(f => new EmbedField
                    {
                        Name = f.Name,
                        Value = f.Value,
                        Inline = f.Inline
                    }).ToList(),
                    Title = e.Title,
                    Type = e.Type,
                    AuthorName = e.AuthorName,
                    ContainsFooter = e.ContainsFooter,
                    ImageInfo = e.ImageInfo,
                    ProviderName = e.ProviderName,
                    ThumbnailInfo = e.ThumbnailInfo,
                    VideoInfo = e.VideoInfo
                }).ToList()
            }).FirstOrDefaultAsync();
    }

    private async Task<InteractionCommandDetail?> CreateInteractionCommandDetailAsync(LogItem header)
    {
        var command = await GetBaseQuery<InteractionCommand>(header).FirstOrDefaultAsync();
        if (command is null)
            return null;

        return new InteractionCommandDetail
        {
            CommandError = command.CommandError,
            Duration = command.Duration,
            Exception = command.Exception,
            Locale = command.Locale,
            Parameters = command.Parameters.ConvertAll(p => new InteractionCommandParameter
            {
                Name = p.Name,
                Value = p.Value,
                Type = p.Type
            }),
            ErrorReason = command.ErrorReason,
            FullName = $"{command.Name} ({command.ModuleName}/{command.MethodName})",
            HasResponded = command.HasResponded,
            IsSuccess = command.IsSuccess,
            IsValidToken = command.IsValidToken
        };
    }

    private async Task<ThreadDeletedDetail?> CreateThreadDeletedDetailAsync(LogItem header)
    {
        return await GetBaseQuery<ThreadDeleted>(header)
            .Select(o => new ThreadDeletedDetail
            {
                Name = o.ThreadInfo.ThreadName,
                Type = o.ThreadInfo.Type,
                SlowMode = o.ThreadInfo.SlowMode,
                ArchivedDuration = o.ThreadInfo.ArchiveDuration,
                IsArchived = o.ThreadInfo.IsArchived,
                IsLocked = o.ThreadInfo.IsLocked
            })
            .FirstOrDefaultAsync();
    }

    private async Task<JobExecutionDetail?> CreateJobExecutionDetailAsync(LogItem header)
    {
        return await GetBaseQuery<JobExecution>(header)
            .Select(o => new JobExecutionDetail
            {
                Result = o.Result,
                EndAt = o.EndAt,
                JobName = o.JobName,
                StartAt = o.StartAt,
                WasError = o.WasError,
                StartUserId = o.StartUserId
            }).FirstOrDefaultAsync();
    }

    private async Task<ApiRequestDetail?> CreateApiRequestDetailAsync(LogItem header)
    {
        return await GetBaseQuery<ApiRequest>(header)
            .Select(o => new ApiRequestDetail
            {
                StartAt = o.StartAt,
                Parameters = o.Parameters,
                Result = o.Result,
                EndAt = o.EndAt,
                Headers = o.Headers,
                Identification = o.Identification,
                Ip = o.Ip,
                Language = o.Language,
                Path = $"{o.Method} {o.Path}",
                ActionName = o.ActionName,
                ControllerName = o.ControllerName,
                TemplatePath = $"{o.Method} {o.TemplatePath}",
                ApiGroupName = o.ApiGroupName,
                Role = o.Role,
                ForwardedIp = o.ForwardedIp
            }).FirstOrDefaultAsync();
    }

    private async Task<ThreadUpdatedDetail?> CreateThreaduUpdatedDetailAsync(LogItem header)
    {
        var threadInfos = await GetBaseQuery<ThreadUpdated>(header)
            .Select(o => new
            {
                TagsBefore = o.Before.Tags,
                TagsAfter = o.After.Tags
            })
            .FirstOrDefaultAsync();
        if (threadInfos is null)
            return null;

        var result = new ThreadUpdatedDetail
        {
            Tags = new Diff<List<string>>(threadInfos.TagsBefore, threadInfos.TagsAfter).NullIfEquals()
        };

        return ModelHelper.IsModelEmpty(result) ? null : result;
    }

    private async Task<RoleDeletedDetail?> CreateRoleDeletedDetailAsync(LogItem header)
    {
        return await GetBaseQuery<RoleDeleted>(header).Select(o => o.RoleInfo).Select(o => new RoleDeletedDetail
        {
            Color = o.Color,
            IconId = o.IconId,
            IsHoisted = o.IsHoisted,
            IsMentionable = o.IsMentionable,
            Name = o.Name,
            Permissions = o.Permissions,
            RoleId = o.RoleId
        }).FirstOrDefaultAsync();
    }
}

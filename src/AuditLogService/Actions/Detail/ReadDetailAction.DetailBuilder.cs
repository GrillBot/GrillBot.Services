using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Core.Extensions;
using AuditLogService.Core.Helpers;
using AuditLogService.Models.Response.Detail;
using Discord;
using GrillBot.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using EmbedField = AuditLogService.Models.Response.Detail.EmbedField;
using InteractionCommandParameter = AuditLogService.Models.Response.Detail.InteractionCommandParameter;

namespace AuditLogService.Actions.Detail;

public partial class ReadDetailAction
{
    private async Task<TData?> CreateDetailAsync<TEntity, TData>(LogItem header, Expression<Func<TEntity, TData>> projection) where TEntity : ChildEntityBase where TData : class
    {
        var query = DbContext.Set<TEntity>()
            .Where(o => o.LogItemId == header.Id)
            .AsNoTracking()
            .Select(projection);

        return await ContextHelper.ReadFirstOrDefaultEntityAsync(query);
    }

    private Task<MessageDetail?> CreateMessageDetailAsync(LogItem header)
    {
        return CreateDetailAsync<Core.Entity.LogMessage, MessageDetail>(header, entity => new MessageDetail
        {
            Source = entity.Source,
            SourceAppName = entity.SourceAppName,
            Text = entity.Message
        });
    }

    private async Task<ChannelUpdatedDetail?> CreateChannelUpdatedDetailAsync(LogItem header)
    {
        var channelInfos = await CreateDetailAsync<ChannelUpdated, Tuple<ChannelInfo, ChannelInfo>>(header, entity => Tuple.Create(entity.Before, entity.After));
        if (channelInfos is null)
            return null;

        var before = channelInfos.Item1;
        var after = channelInfos.Item2;

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

    private async Task<OverwriteDetail?> CreateOverwriteDetailAsync(LogItem header)
    {
        var overwriteInfo = header.Type switch
        {
            LogType.OverwriteCreated => await CreateDetailAsync<OverwriteCreated, OverwriteInfo>(header, entity => entity.OverwriteInfo),
            LogType.OverwriteDeleted => await CreateDetailAsync<OverwriteDeleted, OverwriteInfo>(header, entity => entity.OverwriteInfo),
            _ => null
        };

        if (overwriteInfo is null)
            return null;

        var perms = new OverwritePermissions(overwriteInfo.AllowValue, overwriteInfo.DenyValue);

        return new OverwriteDetail
        {
            Allow = perms.ToAllowList().ConvertAll(o => o.ToString()),
            Deny = perms.ToDenyList().ConvertAll(o => o.ToString()),
            TargetId = overwriteInfo.TargetId,
            TargetType = overwriteInfo.Target
        };
    }

    private async Task<OverwriteUpdatedDetail?> CreateOverwriteUpdatedDetailAsync(LogItem header)
    {
        var overwriteInfos = await CreateDetailAsync<OverwriteUpdated, Tuple<OverwriteInfo, OverwriteInfo>>(header, entity => Tuple.Create(entity.Before, entity.After));
        if (overwriteInfos is null)
            return null;

        var before = overwriteInfos.Item1;
        var after = overwriteInfos.Item2;

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
        var memberInfos = await CreateDetailAsync<MemberUpdated, Tuple<MemberInfo, MemberInfo>>(header, entity => Tuple.Create(entity.Before, entity.After));
        if (memberInfos is null)
            return null;

        var before = memberInfos.Item1;
        var after = memberInfos.Item2;

        var result = new MemberUpdatedDetail
        {
            IsMuted = new Diff<bool?>(before.IsMuted, after.IsMuted).NullIfEquals(),
            Flags = new Diff<int?>(before.Flags, after.Flags).NullIfEquals(),
            SelfUnverifyMinimalTime = new Diff<string?>(before.SelfUnverifyMinimalTime, after.SelfUnverifyMinimalTime).NullIfEquals(),
            Nickname = new Diff<string?>(before.Nickname, after.Nickname).NullIfEquals(),
            IsDeaf = new Diff<bool?>(before.IsDeaf, after.IsDeaf).NullIfEquals(),
            UserId = before.UserId,
            PointsDeactivated = new Diff<bool?>(before.PointsDeactivated, after.PointsDeactivated).NullIfEquals()
        };

        return ModelHelper.IsModelEmpty(result) ? null : result;
    }

    private async Task<GuildUpdatedDetail?> CreateGuildUpdatedDetailAsync(LogItem header)
    {
        var guildInfos = await CreateDetailAsync<GuildUpdated, Tuple<GuildInfo, GuildInfo>>(header, entity => Tuple.Create(entity.Before, entity.After));
        if (guildInfos is null)
            return null;

        var before = guildInfos.Item1;
        var after = guildInfos.Item2;

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

    private Task<MessageDeletedDetail?> CreateMessageDeletedDetailAsync(LogItem header)
    {
        return CreateDetailAsync<MessageDeleted, MessageDeletedDetail>(header, entity => new MessageDeletedDetail
        {
            AuthorId = entity.AuthorId,
            Content = entity.Content,
            Embeds = entity.Embeds.Select(e => new EmbedDetail
            {
                AuthorName = e.AuthorName,
                ContainsFooter = e.ContainsFooter,
                Fields = e.Fields.Select(f => new EmbedField
                {
                    Inline = f.Inline,
                    Name = f.Name,
                    Value = f.Value
                }).ToList(),
                ImageInfo = e.ImageInfo,
                ProviderName = e.ProviderName,
                ThumbnailInfo = e.ThumbnailInfo,
                Title = e.Title,
                Type = e.Type,
                VideoInfo = e.VideoInfo
            }).ToList(),
            MessageCreatedAt = entity.MessageCreatedAt
        });
    }

    private Task<MessageEditedDetail?> CreateMessageEditedDetailAsync(LogItem header)
    {
        return CreateDetailAsync<MessageEdited, MessageEditedDetail>(header, entity => new MessageEditedDetail
        {
            ContentAfter = entity.ContentAfter,
            ContentBefore = entity.ContentBefore,
            JumpLink = entity.JumpUrl
        });
    }

    private async Task<InteractionCommandDetail?> CreateInteractionCommandDetailAsync(LogItem header)
    {
        var detail = await CreateDetailAsync<InteractionCommand, InteractionCommandDetail>(header, entity => new InteractionCommandDetail
        {
            CommandError = entity.CommandError,
            Duration = entity.Duration,
            ErrorReason = entity.ErrorReason,
            Exception = entity.Exception,
            FullName = $"{entity.Name} ({entity.ModuleName}/{entity.MethodName})",
            HasResponded = entity.HasResponded,
            IsSuccess = entity.IsSuccess,
            IsValidToken = entity.IsValidToken,
            Locale = entity.Locale
        });

        if (detail is null)
            return null;

        var parameters = await ContextHelper.ReadFirstOrDefaultEntityAsync(
            DbContext.InteractionCommands.Where(o => o.LogItemId == header.Id).Select(o => o.Parameters),
            CancellationToken
        );

        detail.Parameters = parameters?.ConvertAll(o => new InteractionCommandParameter
        {
            Name = o.Name,
            Type = o.Type,
            Value = o.Value
        }) ?? [];

        return detail;
    }

    private Task<ThreadDeletedDetail?> CreateThreadDeletedDetailAsync(LogItem header)
    {
        return CreateDetailAsync<ThreadDeleted, ThreadDeletedDetail>(header, entity => new ThreadDeletedDetail
        {
            ArchivedDuration = entity.ThreadInfo.ArchiveDuration,
            IsArchived = entity.ThreadInfo.IsArchived,
            IsLocked = entity.ThreadInfo.IsLocked,
            Name = entity.ThreadInfo.ThreadName,
            SlowMode = entity.ThreadInfo.SlowMode,
            Type = entity.ThreadInfo.Type
        });
    }

    private Task<JobExecutionDetail?> CreateJobExecutionDetailAsync(LogItem header)
    {
        return CreateDetailAsync<JobExecution, JobExecutionDetail>(header, entity => new JobExecutionDetail
        {
            EndAt = entity.EndAt,
            JobName = entity.JobName,
            Result = entity.Result,
            StartAt = entity.StartAt,
            StartUserId = entity.StartUserId,
            WasError = entity.WasError
        });
    }

    private Task<ApiRequestDetail?> CreateApiRequestDetailAsync(LogItem header)
    {
        return CreateDetailAsync<ApiRequest, ApiRequestDetail>(header, entity => new ApiRequestDetail
        {
            ActionName = entity.ActionName,
            ApiGroupName = entity.ApiGroupName,
            ControllerName = entity.ControllerName.Replace("Controller", ""),
            EndAt = entity.EndAt,
            ForwardedIp = entity.ForwardedIp,
            Headers = entity.Headers ?? new(),
            Identification = entity.Identification,
            Ip = entity.Ip,
            Language = entity.Language,
            Parameters = entity.Parameters ?? new(),
            Path = $"{entity.Method} {entity.Path}",
            Result = entity.Result,
            Role = entity.Role,
            StartAt = entity.StartAt,
            TemplatePath = $"{entity.Method} {entity.TemplatePath}"
        });
    }

    private async Task<ThreadUpdatedDetail?> CreateThreaduUpdatedDetailAsync(LogItem header)
    {
        var threadInfos = await CreateDetailAsync<ThreadUpdated, Tuple<List<string>, List<string>>>(
            header, entity => Tuple.Create(entity.Before.Tags ?? new(), entity.After.Tags ?? new())
        );
        if (threadInfos is null)
            return null;

        var result = new ThreadUpdatedDetail
        {
            Tags = new Diff<List<string>>(threadInfos.Item1, threadInfos.Item2).NullIfEquals()
        };

        return ModelHelper.IsModelEmpty(result) ? null : result;
    }

    private async Task<RoleDeletedDetail?> CreateRoleDeletedDetailAsync(LogItem header)
    {
        return await CreateDetailAsync<RoleDeleted, RoleDeletedDetail>(header, entity => new RoleDeletedDetail
        {
            Color = entity.RoleInfo.Color,
            IconId = entity.RoleInfo.IconId,
            IsHoisted = entity.RoleInfo.IsHoisted,
            IsMentionable = entity.RoleInfo.IsMentionable,
            Name = entity.RoleInfo.Name,
            Permissions = entity.RoleInfo.Permissions ?? new(),
            RoleId = entity.RoleInfo.RoleId
        });
    }
}

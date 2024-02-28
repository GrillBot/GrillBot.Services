using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Response.Search;
using Discord;
using GrillBot.Core.Extensions;
using GrillBot.Core.Models.Pagination;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AuditLogService.Actions.Search;

public partial class SearchItemsAction
{
    private async Task<PaginatedResponse<LogListItem>> MapAsync(PaginatedResponse<LogItem> headers)
        => await PaginatedResponse<LogListItem>.CopyAndMapAsync(headers, MapAsync);

    private async Task<LogListItem> MapAsync(LogItem item)
    {
        var result = new LogListItem
        {
            GuildId = item.GuildId,
            Id = item.Id,
            Type = item.Type,
            ChannelId = item.ChannelId,
            CreatedAt = item.CreatedAt,
            UserId = item.UserId,
            Files = item.Files.Select(o => new Models.Response.Search.File
            {
                Filename = o.Filename,
                Size = o.Size
            }).ToList()
        };

        switch (item.Type)
        {
            case LogType.Info or LogType.Warning or LogType.Error:
                await SetTextPreviewAsync(result);
                break;
            case LogType.ChannelCreated or LogType.ChannelDeleted:
                await SetChannelPreviewAsync(result);
                break;
            case LogType.ChannelUpdated:
                await SetChannelUpdatedPreviewAsync(result);
                break;
            case LogType.EmoteDeleted:
                await SetEmoteDeletedPreviewAsync(result);
                break;
            case LogType.OverwriteCreated or LogType.OverwriteDeleted:
                await SetOverwritePreviewAsync(result);
                break;
            case LogType.OverwriteUpdated:
                await SetOverwriteUpdatedPreviewAsync(result);
                break;
            case LogType.Unban:
                await SetUnbanPreviewAsync(result);
                break;
            case LogType.MemberUpdated:
                await SetMemberUpdatedPreviewAsync(result);
                break;
            case LogType.MemberRoleUpdated:
                await SetMemberRoleUpdatedPreviewAsync(result);
                break;
            case LogType.GuildUpdated:
                await SetGuildUpdatedPreviewAsync(result);
                break;
            case LogType.UserLeft:
                await SetUserLeftPreviewAsync(result);
                break;
            case LogType.UserJoined:
                await SetUserJoinedPreviewAsync(result);
                break;
            case LogType.MessageEdited:
                await SetMessageEditedPreviewAsync(result);
                break;
            case LogType.MessageDeleted:
                await SetMesasgeDeletedPreviewAsync(result);
                break;
            case LogType.InteractionCommand:
                await SetInteractionCommandPreviewAsync(result);
                break;
            case LogType.ThreadDeleted:
                await SetThreadDeletedPreviewAsync(result);
                break;
            case LogType.JobCompleted:
                await SetJobPreviewAsync(result);
                break;
            case LogType.Api:
                await SetApiPreviewAsync(result);
                break;
            case LogType.ThreadUpdated:
                await SetThreadUpdatedPreviewAsync(result);
                break;
            case LogType.RoleDeleted:
                await SetRoleDeletedPreviewAsync(result);
                break;
        }

        return result;
    }

    private async Task SetTextPreviewAsync(LogListItem result)
    {
        var preview = await CreatePreviewAsync<Core.Entity.LogMessage, TextPreview>(result, entity => new TextPreview
        {
            FullSource = $"{entity.SourceAppName}/{entity.Source}",
            Message = entity.Message
        });

        if (preview is null)
            return;

        const int maxLength = 1000;
        result.IsDetailAvailable = preview.Message.Length > maxLength;

        preview.Message = preview.Message.Cut(maxLength) ?? "";
        result.Preview = preview;
    }

    private async Task SetChannelPreviewAsync(LogListItem result)
    {
        var channel = result.Type switch
        {
            LogType.ChannelCreated => await CreatePreviewAsync<ChannelCreated, ChannelInfo>(result, entity => entity.ChannelInfo),
            LogType.ChannelDeleted => await CreatePreviewAsync<ChannelDeleted, ChannelInfo>(result, entity => entity.ChannelInfo),
            _ => null
        };

        if (channel is null)
            return;

        result.Preview = new ChannelPreview
        {
            Bitrate = channel.Bitrate,
            IsNsfw = channel.IsNsfw,
            Type = (channel.ChannelType ?? ChannelType.Text).ToString(),
            Name = channel.ChannelName ?? "",
            Slowmode = channel.SlowMode
        };
    }

    private async Task SetChannelUpdatedPreviewAsync(LogListItem result)
    {
        result.IsDetailAvailable = true;
        result.Preview = await CreatePreviewAsync<ChannelUpdated, ChannelUpdatedPreview>(result, o => new ChannelUpdatedPreview
        {
            Position = o.Before.Position != o.After.Position,
            SlowMode = o.Before.SlowMode != o.After.SlowMode,
            Bitrate = o.Before.Bitrate != o.After.SlowMode,
            Name = o.Before.ChannelName != o.After.ChannelName,
            IsNsfw = o.Before.IsNsfw != o.After.IsNsfw,
            Flags = o.Before.Flags != o.After.Flags,
            Topic = o.Before.Topic != o.After.Topic
        });
    }

    private async Task SetEmoteDeletedPreviewAsync(LogListItem result)
    {
        result.Preview = await CreatePreviewAsync<DeletedEmote, EmoteDeletedPreview>(result, entity => new EmoteDeletedPreview
        {
            Id = entity.EmoteId,
            Name = entity.EmoteName
        });
    }

    private async Task SetOverwritePreviewAsync(LogListItem result)
    {
        var overwrite = result.Type switch
        {
            LogType.OverwriteCreated => await CreatePreviewAsync<OverwriteCreated, OverwriteInfo>(result, entity => entity.OverwriteInfo),
            LogType.OverwriteDeleted => await CreatePreviewAsync<OverwriteDeleted, OverwriteInfo>(result, entity => entity.OverwriteInfo),
            _ => null
        };

        if (overwrite is null)
            return;

        var perms = new OverwritePermissions(overwrite.AllowValue, overwrite.DenyValue);
        result.Preview = new OverwritePreview
        {
            Allow = perms.ToAllowList().ConvertAll(o => o.ToString()),
            TargetId = overwrite.TargetId,
            Deny = perms.ToDenyList().ConvertAll(o => o.ToString()),
            TargetType = overwrite.Target
        };
    }

    private async Task SetOverwriteUpdatedPreviewAsync(LogListItem result)
    {
        result.IsDetailAvailable = true;
        result.Preview = await CreatePreviewAsync<OverwriteUpdated, OverwriteUpdatedPreview>(result, entity => new OverwriteUpdatedPreview
        {
            TargetId = entity.After.TargetId,
            TargetType = entity.After.Target
        });
    }

    private async Task SetUnbanPreviewAsync(LogListItem result)
    {
        result.Preview = await CreatePreviewAsync<Unban, UnbanPreview>(result, entity => new UnbanPreview
        {
            UserId = entity.UserId
        });
    }

    private async Task SetMemberUpdatedPreviewAsync(LogListItem result)
    {
        result.IsDetailAvailable = true;
        result.Preview = await CreatePreviewAsync<MemberUpdated, MemberUpdatedPreview>(result, o => new MemberUpdatedPreview
        {
            NicknameChanged = o.Before.Nickname != o.After.Nickname,
            UserId = o.Before.UserId,
            FlagsChanged = o.Before.Flags != o.After.Flags,
            VoiceMuteChanged = o.Before.IsDeaf != o.After.IsDeaf || o.Before.IsMuted != o.After.IsMuted,
            SelfUnverifyMinimalTimeChange = o.Before.SelfUnverifyMinimalTime != o.After.SelfUnverifyMinimalTime
        });
    }

    private async Task SetMemberRoleUpdatedPreviewAsync(LogListItem result)
    {
        List<MemberRoleUpdated> roles;
        using (CreateCounter("Database"))
            roles = await Context.MemberRoleUpdatedItems.AsNoTracking().Where(o => o.LogItemId == result.Id).ToListAsync();

        if (roles.Count == 0)
            return;

        result.Preview = new MemberRoleUpdatedPreview
        {
            UserId = roles[0].UserId,
            ModifiedRoles = roles.DistinctBy(o => o.RoleName).ToDictionary(o => o.RoleName, o => o.IsAdded)
        };
    }

    private async Task SetGuildUpdatedPreviewAsync(LogListItem result)
    {
        result.IsDetailAvailable = true;
        result.Preview = await CreatePreviewAsync<GuildUpdated, GuildUpdatedPreview>(result, o => new GuildUpdatedPreview
        {
            SystemChannelFlags = o.Before.SystemChannelFlags != o.After.SystemChannelFlags,
            Name = o.Before.Name != o.After.Name,
            DefaultMessageNotifications = o.Before.DefaultMessageNotifications != o.After.DefaultMessageNotifications,
            Description = o.Before.Description != o.After.Description,
            Features = o.Before.Features != o.After.Features,
            AfkTimeout = o.Before.AfkTimeout != o.After.AfkTimeout,
            BannerId = o.Before.BannerId != o.After.BannerId,
            IconId = o.Before.IconId != o.After.IconId,
            MfaLevel = o.Before.MfaLevel != o.After.MfaLevel,
            NsfwLevel = o.Before.NsfwLevel != o.After.NsfwLevel,
            PremiumTier = o.Before.PremiumTier != o.After.PremiumTier,
            SplashId = o.Before.SplashId != o.After.SplashId,
            VanityUrl = o.Before.VanityUrl != o.After.VanityUrl,
            VerificationLevel = o.Before.VerificationLevel != o.After.VerificationLevel,
            AfkChannelId = o.Before.AfkChannelId != o.After.AfkChannelId,
            DiscoverySplashId = o.Before.DiscoverySplashId != o.After.DiscoverySplashId,
            ExplicitContentFilter = o.Before.ExplicitContentFilter != o.After.ExplicitContentFilter,
            RulesChannelId = o.Before.RulesChannelId != o.After.RulesChannelId,
            SystemChannelId = o.Before.SystemChannelId != o.After.SystemChannelId,
            PublicUpdatesChannelId = o.Before.PublicUpdatesChannelId != o.After.PublicUpdatesChannelId
        });
    }

    private async Task SetUserLeftPreviewAsync(LogListItem result)
    {
        result.Preview = await CreatePreviewAsync<UserLeft, UserLeftPreview>(result, entity => new UserLeftPreview
        {
            BanReason = entity.BanReason,
            IsBan = entity.IsBan,
            MemberCount = entity.MemberCount,
            UserId = entity.UserId
        });
    }

    private async Task SetUserJoinedPreviewAsync(LogListItem result)
    {
        result.Preview = await CreatePreviewAsync<UserJoined, UserJoinedPreview>(result, entity => new UserJoinedPreview
        {
            MemberCount = entity.MemberCount
        });
    }

    private async Task SetMessageEditedPreviewAsync(LogListItem result)
    {
        result.Preview = await CreatePreviewAsync<MessageEdited, MessageEditedPreview>(result, entity => new MessageEditedPreview
        {
            ContentAfter = entity.ContentAfter,
            ContentBefore = entity.ContentBefore,
            JumpUrl = entity.JumpUrl
        });
    }

    private async Task SetMesasgeDeletedPreviewAsync(LogListItem result)
    {
        var preview = await CreatePreviewAsync<MessageDeleted, MessageDeletedPreview>(result, entity => new MessageDeletedPreview
        {
            AuthorId = entity.AuthorId,
            Content = entity.Content,
            MessageCreatedAt = entity.MessageCreatedAt,
            Embeds = entity.Embeds.Select(e => new EmbedPreview
            {
                AuthorName = e.AuthorName,
                ContainsFooter = e.ContainsFooter,
                FieldsCount = e.Fields.Count,
                ImageInfo = e.ImageInfo,
                ProviderName = e.ProviderName,
                ThumbnailInfo = e.ThumbnailInfo,
                Title = e.Title,
                Type = e.Type,
                VideoInfo = e.VideoInfo
            }).ToList()
        });

        result.Preview = preview;

        if (preview is not null)
            result.IsDetailAvailable = preview.Embeds.Exists(o => o.FieldsCount > 0);
    }

    private async Task SetInteractionCommandPreviewAsync(LogListItem result)
    {
        result.IsDetailAvailable = true;
        result.Preview = await CreatePreviewAsync<InteractionCommand, InteractionCommandPreview>(result, entity => new InteractionCommandPreview
        {
            HasResponded = entity.HasResponded,
            IsSuccess = entity.IsSuccess,
            Name = $"{entity.Name} ({entity.ModuleName}/{entity.MethodName})",
            CommandError = entity.CommandError
        });
    }

    private async Task SetThreadDeletedPreviewAsync(LogListItem result)
    {
        result.IsDetailAvailable = true;
        result.Preview = await CreatePreviewAsync<ThreadDeleted, ThreadDeletedPreview>(result, entity => new ThreadDeletedPreview
        {
            Name = entity.ThreadInfo.ThreadName
        });
    }

    private async Task SetJobPreviewAsync(LogListItem result)
    {
        result.IsDetailAvailable = true;
        result.Preview = await CreatePreviewAsync<JobExecution, JobPreview>(result, entity => new JobPreview
        {
            EndAt = entity.EndAt,
            JobName = entity.JobName,
            StartAt = entity.StartAt,
            WasError = entity.WasError
        });
    }

    private async Task SetApiPreviewAsync(LogListItem result)
    {
        result.IsDetailAvailable = true;
        result.Preview = await CreatePreviewAsync<ApiRequest, ApiPreview>(result, entity => new ApiPreview
        {
            Action = $"{entity.ControllerName}.{entity.ActionName}",
            Duration = (int)entity.Duration,
            Path = $"{entity.Method} {entity.TemplatePath} (API {entity.ApiGroupName})",
            Result = entity.Result
        });
    }

    private async Task SetThreadUpdatedPreviewAsync(LogListItem result)
    {
        var info = await CreatePreviewAsync<ThreadUpdated, Tuple<List<string>, List<string>>>(result, entity => Tuple.Create(entity.Before.Tags, entity.After.Tags));

        var tagsBefore = info?.Item1 ?? new List<string>();
        var tagsAfter = info?.Item2 ?? new List<string>();

        result.IsDetailAvailable = true;
        result.Preview = new ThreadUpdatedPreview
        {
            TagsChanged = !tagsBefore.IsSequenceEqual(tagsAfter)
        };
    }

    private async Task SetRoleDeletedPreviewAsync(LogListItem result)
    {
        result.IsDetailAvailable = true;
        result.Preview = await CreatePreviewAsync<RoleDeleted, RoleDeletedPreview>(result, entity => new RoleDeletedPreview
        {
            Name = entity.RoleInfo.Name,
            RoleId = entity.RoleInfo.RoleId
        });
    }

    private async Task<TPreview?> CreatePreviewAsync<TEntity, TPreview>(LogListItem item, Expression<Func<TEntity, TPreview>> projection) where TEntity : ChildEntityBase
    {
        var query = Context.Set<TEntity>().AsNoTracking()
            .Where(o => o.LogItemId == item.Id)
            .Select(projection);

        using (CreateCounter("Database"))
            return await query.FirstOrDefaultAsync();
    }
}

using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Response.Search;
using Discord;
using GrillBot.Core.Extensions;
using GrillBot.Core.Models.Pagination;
using Microsoft.EntityFrameworkCore;

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
        }

        return result;
    }

    private async Task SetTextPreviewAsync(LogListItem result)
    {
        var logMessage = await Context.LogMessages.AsNoTracking()
            .Where(o => o.LogItemId == result.Id)
            .Select(o => o.Message)
            .FirstOrDefaultAsync();
        logMessage ??= "";

        const int maxLength = 1000;
        result.IsDetailAvailable = logMessage.Length > maxLength;
        result.Preview = new TextPreview
        {
            Message = logMessage.Cut(maxLength) ?? ""
        };
    }

    private async Task SetChannelPreviewAsync(LogListItem result)
    {
        var baseQuery = result.Type switch
        {
            LogType.ChannelCreated => Context.ChannelCreatedItems.AsNoTracking().Where(o => o.LogItemId == result.Id).Select(o => o.ChannelInfo),
            LogType.ChannelDeleted => Context.ChannelDeletedItems.AsNoTracking().Where(o => o.LogItemId == result.Id).Select(o => o.ChannelInfo),
            _ => null
        };

        if (baseQuery is null)
            return;

        var channel = await baseQuery
            .Select(o => new { o.ChannelName, o.ChannelType, o.SlowMode, o.IsNsfw, o.Bitrate })
            .FirstOrDefaultAsync();
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
        var query = Context.ChannelUpdatedItems.AsNoTracking()
            .Where(o => o.LogItemId == result.Id)
            .Select(o => new ChannelUpdatedPreview
            {
                Position = o.Before.Position != o.After.Position,
                SlowMode = o.Before.SlowMode != o.After.SlowMode,
                Bitrate = o.Before.Bitrate != o.After.SlowMode,
                Name = o.Before.ChannelName != o.After.ChannelName,
                IsNsfw = o.Before.IsNsfw != o.After.IsNsfw,
                Flags = o.Before.Flags != o.After.Flags,
                Topic = o.Before.Topic != o.After.Topic
            });

        result.IsDetailAvailable = true;
        result.Preview = await query.FirstOrDefaultAsync();
    }

    private async Task SetEmoteDeletedPreviewAsync(LogListItem result)
    {
        var query = Context.DeletedEmotes.AsNoTracking()
            .Where(o => o.LogItemId == result.Id)
            .Select(o => new EmoteDeletedPreview
            {
                Id = o.EmoteId,
                Name = o.EmoteName
            });

        result.Preview = await query.FirstOrDefaultAsync();
    }

    private async Task SetOverwritePreviewAsync(LogListItem result)
    {
        var baseQuery = result.Type switch
        {
            LogType.OverwriteCreated => Context.OverwriteCreatedItems.AsNoTracking().Where(o => o.LogItemId == result.Id).Select(o => o.OverwriteInfo),
            LogType.OverwriteDeleted => Context.OverwriteDeletedItems.AsNoTracking().Where(o => o.LogItemId == result.Id).Select(o => o.OverwriteInfo),
            _ => null
        };

        if (baseQuery is null)
            return;

        var overwrite = await baseQuery
            .Select(o => new { o.TargetId, o.Target, o.AllowValue, o.DenyValue })
            .FirstOrDefaultAsync();
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
        var query = Context.OverwriteUpdatedItems.AsNoTracking()
            .Where(o => o.LogItemId == result.Id)
            .Select(o => new OverwriteUpdatedPreview
            {
                TargetId = o.After.TargetId,
                TargetType = o.After.Target
            });

        result.IsDetailAvailable = true;
        result.Preview = await query.FirstOrDefaultAsync();
    }

    private async Task SetUnbanPreviewAsync(LogListItem result)
    {
        var query = Context.Unbans.AsNoTracking()
            .Where(o => o.LogItemId == result.Id)
            .Select(o => new UnbanPreview { UserId = o.UserId });

        result.Preview = await query.FirstOrDefaultAsync();
    }

    private async Task SetMemberUpdatedPreviewAsync(LogListItem result)
    {
        var query = Context.MemberUpdatedItems.AsNoTracking()
            .Where(o => o.LogItemId == result.Id)
            .Select(o => new MemberUpdatedPreview
            {
                NicknameChanged = o.Before.Nickname != o.After.Nickname,
                UserId = o.Before.UserId,
                FlagsChanged = o.Before.Flags != o.After.Flags,
                VoiceMuteChanged = o.Before.IsDeaf != o.After.IsDeaf || o.Before.IsMuted != o.After.IsMuted,
                SelfUnverifyMinimalTimeChange = o.Before.SelfUnverifyMinimalTime != o.After.SelfUnverifyMinimalTime
            });

        result.IsDetailAvailable = true;
        result.Preview = await query.FirstOrDefaultAsync();
    }

    private async Task SetMemberRoleUpdatedPreviewAsync(LogListItem result)
    {
        var roles = await Context.MemberRoleUpdatedItems.AsNoTracking()
            .Where(o => o.LogItemId == result.Id)
            .ToListAsync();
        if (roles.Count == 0)
            return;

        result.Preview = new MemberRoleUpdatedPreview
        {
            UserId = roles.First().UserId,
            ModifiedRoles = roles.DistinctBy(o => o.RoleName).ToDictionary(o => o.RoleName, o => o.IsAdded)
        };
    }

    private async Task SetGuildUpdatedPreviewAsync(LogListItem result)
    {
        var query = Context.GuildUpdatedItems.AsNoTracking()
            .Where(o => o.LogItemId == result.Id)
            .Select(o => new GuildUpdatedPreview
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
                OwnerId = o.Before.OwnerId != o.After.OwnerId,
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

        result.IsDetailAvailable = true;
        result.Preview = await query.FirstOrDefaultAsync();
    }

    private async Task SetUserLeftPreviewAsync(LogListItem result)
    {
        var query = Context.UserLeftItems.AsNoTracking()
            .Where(o => o.LogItemId == result.Id)
            .Select(o => new UserLeftPreview
            {
                BanReason = o.BanReason,
                UserId = o.UserId,
                IsBan = o.IsBan,
                MemberCount = o.MemberCount
            });

        result.Preview = await query.FirstOrDefaultAsync();
    }

    private async Task SetUserJoinedPreviewAsync(LogListItem result)
    {
        var query = Context.UserJoinedItems.AsNoTracking()
            .Where(o => o.LogItemId == result.Id)
            .Select(o => new UserJoinedPreview { MemberCount = o.MemberCount });

        result.Preview = await query.FirstOrDefaultAsync();
    }

    private async Task SetMessageEditedPreviewAsync(LogListItem result)
    {
        var query = Context.MessageEditedItems.AsNoTracking()
            .Where(o => o.LogItemId == result.Id)
            .Select(o => new MessageEditedPreview
            {
                ContentAfter = o.ContentAfter,
                ContentBefore = o.ContentBefore,
                JumpUrl = o.JumpUrl
            });

        result.Preview = await query.FirstOrDefaultAsync();
    }

    private async Task SetMesasgeDeletedPreviewAsync(LogListItem result)
    {
        var query = Context.MessageDeletedItems.AsNoTracking()
            .Where(o => o.LogItemId == result.Id)
            .Select(o => new MessageDeletedPreview
            {
                Content = o.Content,
                AuthorId = o.AuthorId,
                MessageCreatedAt = o.MessageCreatedAt,
                Embeds = o.Embeds.Select(e => new EmbedPreview
                {
                    Title = e.Title,
                    Type = e.Type,
                    AuthorName = e.AuthorName,
                    ContainsFooter = e.ContainsFooter,
                    FieldsCount = e.Fields.Count,
                    ImageInfo = e.ImageInfo,
                    ProviderName = e.ProviderName,
                    ThumbnailInfo = e.ThumbnailInfo,
                    VideoInfo = e.VideoInfo
                }).ToList()
            });

        var preview = await query.FirstOrDefaultAsync();
        result.Preview = preview;

        if (preview is not null)
            result.IsDetailAvailable = preview.Embeds.Exists(o => o.FieldsCount > 0);
    }

    private async Task SetInteractionCommandPreviewAsync(LogListItem result)
    {
        var query = Context.InteractionCommands.AsNoTracking()
            .Where(o => o.LogItemId == result.Id)
            .Select(o => new InteractionCommandPreview
            {
                Name = $"{o.Name} ({o.ModuleName}/{o.MethodName})",
                HasResponded = o.HasResponded,
                IsSuccess = o.IsSuccess
            });

        result.IsDetailAvailable = true;
        result.Preview = await query.FirstOrDefaultAsync();
    }

    private async Task SetThreadDeletedPreviewAsync(LogListItem result)
    {
        var query = Context.ThreadDeletedItems.AsNoTracking()
            .Where(o => o.LogItemId == result.Id)
            .Select(o => new ThreadDeletedPreview { Name = o.ThreadInfo.ThreadName });

        result.IsDetailAvailable = true;
        result.Preview = await query.FirstOrDefaultAsync();
    }

    private async Task SetJobPreviewAsync(LogListItem result)
    {
        var query = Context.JobExecutions.AsNoTracking()
            .Where(o => o.LogItemId == result.Id)
            .Select(o => new JobPreview
            {
                JobName = o.JobName,
                EndAt = o.EndAt,
                StartAt = o.StartAt,
                WasError = o.WasError
            });

        result.IsDetailAvailable = true;
        result.Preview = await query.FirstOrDefaultAsync();
    }

    private async Task SetApiPreviewAsync(LogListItem result)
    {
        var query = Context.ApiRequests.AsNoTracking()
            .Where(o => o.LogItemId == result.Id)
            .Select(o => new ApiPreview
            {
                Action = $"{o.ControllerName}.{o.ActionName}",
                Duration = Convert.ToInt32((o.EndAt - o.StartAt).TotalMilliseconds),
                Path = $"{o.Method} {o.TemplatePath} (API {o.ApiGroupName})",
                Result = o.Result
            });

        result.IsDetailAvailable = true;
        result.Preview = await query.FirstOrDefaultAsync();
    }

    private async Task SetThreadUpdatedPreviewAsync(LogListItem result)
    {
        var info = await Context.ThreadUpdatedItems.AsNoTracking()
            .Where(o => o.LogItemId == result.Id)
            .Select(o => new
            {
                Before = o.Before.Tags,
                After = o.After.Tags
            }).FirstOrDefaultAsync();

        var tagsBefore = info?.Before ?? new List<string>();
        var tagsAfter = info?.After ?? new List<string>();

        result.IsDetailAvailable = true;
        result.Preview = new ThreadUpdatedPreview
        {
            TagsChanged = !tagsBefore.OrderBy(o => o).SequenceEqual(tagsAfter.OrderBy(o => o))
        };
    }
}

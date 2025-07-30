using Discord;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using UnverifyService.Core.Entity;
using UnverifyService.Models.Response;

namespace UnverifyService.Actions;

public class GetActiveUnverifyDetailAction(IServiceProvider serviceProvider) : ApiAction<UnverifyContext>(serviceProvider)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = GetParameter<ulong>(0);
        var userId = GetParameter<ulong>(1);

        var unverifyQuery = DbContext.ActiveUnverifies
            .AsNoTracking()
            .AsSplitQuery()
            .Where(o => o.LogItem.GuildId == guildId && o.LogItem.ToUserId == userId)
            .Select(o => new
            {
                o.Id,
                LogItem = new
                {
                    o.LogItem.LogNumber,
                    FromUserId = o.LogItem.FromUserId.ToString(),
                    SetOperation = new
                    {
                        o.LogItem.SetOperation!.Reason,
                        o.LogItem.SetOperation!.Language,
                        o.LogItem.SetOperation!.KeepMutedRole,
                        Roles = o.LogItem.SetOperation!.Roles.Select(r => new
                        {
                            r.IsKept,
                            RoleId = r.RoleId.ToString()
                        }),
                        Channels = o.LogItem.SetOperation!.Channels.Select(ch => new
                        {
                            ch.IsKept,
                            ChannelId = ch.ChannelId.ToString(),
                            ch.AllowValue,
                            ch.DenyValue
                        })
                    }
                },
                o.StartAtUtc,
                o.EndAtUtc,
            });

        var activeUnverify = await ContextHelper.ReadFirstOrDefaultEntityAsync(unverifyQuery, CancellationToken);
        if (activeUnverify is null)
            return ApiResult.NotFound();

        var result = new UnverifyDetail(
            activeUnverify.Id,
            activeUnverify.LogItem.LogNumber,
            activeUnverify.StartAtUtc,
            activeUnverify.EndAtUtc,
            activeUnverify.LogItem.FromUserId,
            activeUnverify.LogItem.SetOperation.Reason,
            activeUnverify.LogItem.SetOperation.Language,
            activeUnverify.LogItem.SetOperation.KeepMutedRole,
            [.. activeUnverify.LogItem.SetOperation.Roles.Where(o => !o.IsKept).Select(o => o.RoleId)],
            [.. activeUnverify.LogItem.SetOperation.Roles.Where(o => o.IsKept).Select(o => o.RoleId)],
            [.. activeUnverify.LogItem.SetOperation.Channels.Where(o => !o.IsKept).Select(x => new ChannelOverride(
                x.ChannelId,
                [.. new OverwritePermissions(x.AllowValue, x.DenyValue).ToAllowList().Select(o => o.ToString())],
                [.. new OverwritePermissions(x.AllowValue, x.DenyValue).ToDenyList().Select(o => o.ToString())]
            ))],
            [.. activeUnverify.LogItem.SetOperation.Channels.Where(o => o.IsKept).Select(x => new ChannelOverride(
                x.ChannelId,
                [.. new OverwritePermissions(x.AllowValue, x.DenyValue).ToAllowList().Select(o => o.ToString())],
                [.. new OverwritePermissions(x.AllowValue, x.DenyValue).ToDenyList().Select(o => o.ToString())]
            ))]
        );

        return ApiResult.Ok(result);
    }
}

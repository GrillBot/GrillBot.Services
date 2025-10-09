using GrillBot.Core.Extensions;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Services.Common.Exceptions;
using GrillBot.Core.Services.Common.Executor;
using UserMeasures;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using UnverifyService;
using UserManagementService.Core.Entity;
using UserManagementService.Models.Response;

using UnverifyServiceModels = UnverifyService.Models.Response.Users;

namespace UserManagementService.Actions;

public class GetUserInfoAction(
    ICounterManager counterManager,
    UserManagementContext context,
    IServiceClientExecutor<IUnverifyServiceClient> _unverifyClient,
    IServiceClientExecutor<IUserMeasuresServiceClient> _userMeasures
) : ApiAction<UserManagementContext>(counterManager, context)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var userId = GetParameter<ulong>(0);
        var userData = await GetUserDataAsync(userId);
        var unverifyInfo = await GetUnverifyUserInfoAsync(userId);
        var measuresInfo = await GetMeasuresUserInfoAsync(userId);

        if (userData.Count == 0 && unverifyInfo is null && measuresInfo is null)
            return ApiResult.NotFound();

        var result = new UserInfo(
            userId.ToString(),
            CreateGuildUserData(userData, unverifyInfo, measuresInfo),
            unverifyInfo?.SelfUnverifyMinimalTime
        );

        return ApiResult.Ok(result);
    }

    private Task<List<Core.Entity.GuildUser>> GetUserDataAsync(ulong userId)
    {
        var query = DbContext.GuildUsers.AsNoTracking()
            .Include(o => o.Nicknames)
            .Where(o => o.UserId == userId);

        return ContextHelper.ReadEntitiesAsync(query, CancellationToken);
    }

    private async Task<UnverifyServiceModels.UserInfo?> GetUnverifyUserInfoAsync(ulong userId)
    {
        try
        {
            return await _unverifyClient.ExecuteRequestAsync(
                async (client, ctx) => await client.GetUserInfoAsync(userId, ctx.CancellationToken),
                CancellationToken
            );
        }
        catch (ClientNotFoundException)
        {
            return null;
        }
    }

    private async Task<UserMeasures.Models.User.UserInfo?> GetMeasuresUserInfoAsync(ulong userId)
    {
        try
        {
            return await _userMeasures.ExecuteRequestAsync(
                async (client, ctx) => await client.GetUserInfoAsync(userId.ToString(), null, ctx.CancellationToken),
                CancellationToken
            );
        }
        catch (ClientNotFoundException)
        {
            return null;
        }
    }

    private static List<Models.Response.GuildUser> CreateGuildUserData(
        List<Core.Entity.GuildUser> users,
        UnverifyServiceModels.UserInfo? unverifyInfo,
        UserMeasures.Models.User.UserInfo? measuresInfo = null
    )
    {
        var result = new List<Models.Response.GuildUser>();

        var guildIds = users.Select(o => o.GuildId.ToString())
            .Concat(unverifyInfo?.CurrentUnverifies.Keys?.ToArray() ?? [])
            .Concat(unverifyInfo?.SelfUnverifyCount.Keys?.ToArray() ?? [])
            .Concat(unverifyInfo?.UnverifyCount.Keys?.ToArray() ?? [])
            .Concat(measuresInfo?.TimeoutCount.Keys?.ToArray() ?? [])
            .Concat(measuresInfo?.WarningCount.Keys?.ToArray() ?? [])
            .Concat(measuresInfo?.UnverifyCount.Keys?.ToArray() ?? [])
            .Distinct();

        foreach (var guildId in guildIds)
        {
            var guildUser = users.Find(o => o.GuildId == guildId.ToUlong());
            var currentUnverify = unverifyInfo?.CurrentUnverifies.TryGetValue(guildId, out var value) == true ? value : null;
            var selfUnverifyCount = unverifyInfo?.SelfUnverifyCount.TryGetValue(guildId, out var count) == true ? count : 0;
            var unverifyCount = measuresInfo?.UnverifyCount.TryGetValue(guildId, out var uCount) == true ? uCount : 0;
            var timeoutCount = measuresInfo?.TimeoutCount.TryGetValue(guildId, out var tCount) == true ? tCount : 0;
            var warningCount = measuresInfo?.WarningCount.TryGetValue(guildId, out var wCount) == true ? wCount : 0;

            result.Add(new Models.Response.GuildUser(
                guildId,
                guildUser?.CurrentNickname,
                guildUser is not null ? [.. guildUser.Nicknames.OrderBy(o => o.Value).Select(o => o.Value)] : [],
                currentUnverify,
                unverifyCount,
                selfUnverifyCount,
                timeoutCount,
                warningCount
            ));
        }

        return result;
    }
}

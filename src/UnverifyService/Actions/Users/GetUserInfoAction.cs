using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using UnverifyService.Core.Entity;
using UnverifyService.Core.Enums;
using UnverifyService.Models.Response;
using UnverifyService.Models.Response.Users;

namespace UnverifyService.Actions.Users;

public class GetUserInfoAction(IServiceProvider serviceProvider) : ApiAction<UnverifyContext>(serviceProvider)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var userId = GetParameter<ulong>(0);

        var userQuery = DbContext.Users.AsNoTracking().Where(o => o.Id == userId);
        var userEntity = await ContextHelper.ReadFirstOrDefaultEntityAsync(userQuery, CancellationToken);
        if (userEntity is null)
            return ApiResult.NotFound();

        var currentUnverifiesQuery = DbContext.ActiveUnverifies.AsNoTracking()
            .Include(o => o.LogItem.SetOperation).ThenInclude(o => o!.Roles)
            .Include(o => o.LogItem.SetOperation).ThenInclude(o => o!.Channels)
            .Where(o => o.LogItem.ToUserId == userId)
            .AsSplitQuery();

        var currentUnverifies = await ContextHelper.ReadEntitiesAsync(currentUnverifiesQuery, CancellationToken);

        var unverifyCountsQuery = DbContext.LogItems.AsNoTracking()
            .Where(o => o.ToUserId == userId && (o.OperationType == UnverifyOperationType.Unverify || o.OperationType == UnverifyOperationType.SelfUnverify))
            .GroupBy(o => new { o.GuildId, o.OperationType })
            .Select(o => new
            {
                o.Key.GuildId,
                o.Key.OperationType,
                Count = o.Count()
            });

        var unverifyCounts = await ContextHelper.ReadEntitiesAsync(unverifyCountsQuery, CancellationToken);

        var result = new UserInfo(
            userEntity.SelfUnverifyMinimalTime,
            currentUnverifies
                .GroupBy(o => o.LogItem.GuildId)
                .ToDictionary(
                    o => o.Key.ToString(),
                    o =>
                    {
                        var unverify = o.First();
                        return new UnverifyInfo(
                            unverify.StartAtUtc,
                            unverify.EndAtUtc,
                            unverify.LogItem.FromUserId.ToString(),
                            unverify.LogItem.OperationType == UnverifyOperationType.SelfUnverify,
                            unverify.LogItem.SetOperation!.Reason,
                            unverify.LogItem.SetOperation.Roles.Count(o => !o.IsKept),
                            unverify.LogItem.SetOperation.Roles.Count(o => o.IsKept),
                            unverify.LogItem.SetOperation.Channels.Count(o => !o.IsKept),
                            unverify.LogItem.SetOperation.Channels.Count(o => o.IsKept)
                        );
                    }
                ),
            unverifyCounts
                .Where(o => o.OperationType == UnverifyOperationType.SelfUnverify)
                .GroupBy(o => o.GuildId.ToString())
                .ToDictionary(o => o.Key, o => o.Sum(x => x.Count)),
            unverifyCounts
                .Where(o => o.OperationType == UnverifyOperationType.Unverify)
                .GroupBy(o => o.GuildId.ToString())
                .ToDictionary(o => o.Key, o => o.Sum(x => x.Count))
        );

        return ApiResult.Ok(result);
    }
}

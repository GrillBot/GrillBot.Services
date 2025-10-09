using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using UserMeasuresService.Core.Entity;
using UserMeasuresService.Models.User;

namespace UserMeasuresService.Actions.User;

public class GetUserInfo(UserMeasuresContext dbContext, ICounterManager counterManager) : ApiAction<UserMeasuresContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var userId = GetParameter<string>(0);
        var guildId = GetOptionalParameter<string>(1);

        var result = new UserInfo(
            await ComputeCountAsync<MemberWarningItem>(guildId, userId),
            await ComputeCountAsync<UnverifyItem>(guildId, userId),
            await ComputeCountAsync<TimeoutItem>(guildId, userId)
        );

        return ApiResult.Ok(result);
    }

    public Task<Dictionary<string, int>> ComputeCountAsync<TEntity>(string? guildId, string userId) where TEntity : UserMeasureBase
    {
        var baseQuery = DbContext.Set<TEntity>()
            .Where(o => o.UserId == userId)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(guildId))
            baseQuery = baseQuery.Where(o => o.GuildId == guildId);

        var query = baseQuery
            .GroupBy(o => o.GuildId)
            .Select(o => new
            {
                o.Key,
                Count = o.Count()
            });

        return ContextHelper.ReadToDictionaryAsync(query, o => o.Key, o => o.Count, CancellationToken);
    }
}

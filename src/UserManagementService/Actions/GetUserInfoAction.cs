using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using UserManagementService.Core.Entity;
using UserManagementService.Models.Response;

namespace UserManagementService.Actions;

public class GetUserInfoAction(
    ICounterManager counterManager,
    UserManagementContext context
) : ApiAction<UserManagementContext>(counterManager, context)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var userId = GetParameter<ulong>(0);

        var query = DbContext.GuildUsers.AsNoTracking()
            .Where(o => o.UserId == userId)
            .GroupBy(o => o.UserId)
            .Select(o => new UserInfo(
                o.Key.ToString(),
                o.Select(g => new Models.Response.GuildUser(
                    g.GuildId.ToString(),
                    g.CurrentNickname,
                    g.Nicknames
                        .OrderBy(o => o.Value)
                        .Select(n => n.Value)
                        .ToList()
                )).ToList()
            ));

        var user = await ContextHelper.ReadFirstOrDefaultEntityAsync(query);
        return user is null ? ApiResult.NotFound() : ApiResult.Ok(user);
    }
}

using GrillBot.Core.Extensions;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using UserManagementService.Core.Entity;

namespace UserManagementService.Actions;

public class GetGuildUsersWithNicknameAction(
    ICounterManager counterManager,
    UserManagementContext dbContext
) : ApiAction<UserManagementContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = GetParameter<ulong>(0);

        var query = DbContext.GuildUsers.AsNoTracking()
            .Where(o => o.GuildId == guildId && !string.IsNullOrEmpty(o.CurrentNickname))
            .Select(o => new
            {
                UserId = o.UserId.ToString(),
                o.CurrentNickname
            });

        var result = await ContextHelper.ReadToDictionaryAsync(query, o => o.UserId, o => o.CurrentNickname);
        return ApiResult.Ok(result);
    }
}

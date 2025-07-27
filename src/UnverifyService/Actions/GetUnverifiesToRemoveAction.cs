using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using UnverifyService.Core.Entity;
using UnverifyService.Models.Response;

namespace UnverifyService.Actions;

public class GetUnverifiesToRemoveAction(IServiceProvider serviceProvider) : ApiAction<UnverifyContext>(serviceProvider)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var query = DbContext.ActiveUnverifies.AsNoTracking()
            .Where(o => o.EndAtUtc <= DateTime.UtcNow)
            .Select(o => new ScheduleUnverifyRemoveItem(
                o.LogItem.GuildId,
                o.LogItem.ToUserId,
                o.LogItem.SetOperation!.Roles.Count(o => !o.IsKept),
                o.LogItem.SetOperation!.Channels.Count(o => !o.IsKept)
            ));

        var items = await ContextHelper.ReadEntitiesAsync(query, CancellationToken);
        return ApiResult.Ok(items);
    }
}

using GrillBot.Core.Extensions;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models;
using GrillBot.Services.Common.EntityFramework.Extensions;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UnverifyService.Core.Entity;
using UnverifyService.Core.Enums;
using UnverifyService.Models.Request;
using UnverifyService.Models.Response;

namespace UnverifyService.Actions;

public class GetActiveUnverifyListAction(UnverifyContext dbContext, ICounterManager counterManager) : ApiAction<UnverifyContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<ActiveUnverifyListRequest>(0);

        var baseQuery = DbContext.ActiveUnverifies
            .Include(o => o.LogItem.SetOperation)
            .AsNoTracking();

        if (!string.IsNullOrEmpty(request.GuildId))
        {
            var guildId = request.GuildId.ToUlong();
            baseQuery = baseQuery.Where(o => o.LogItem.GuildId == guildId);
        }

        baseQuery = WithSorting(baseQuery, request.Sort);

        var mappedQuery = baseQuery.Select(o => new ActiveUnverifyListItemResponse(
            o.Id,
            o.LogItem.GuildId.ToString(),
            o.LogItem.FromUserId.ToString(),
            o.LogItem.ToUserId.ToString(),
            o.StartAtUtc,
            o.EndAtUtc,
            o.LogItem.OperationType == UnverifyOperationType.SelfUnverify,
            o.LogItem.SetOperation!.Reason,
            o.LogItem.SetOperation!.Language ?? "-",
            o.LogItem.SetOperation!.Roles.Count(r => !r.IsKept),
            o.LogItem.SetOperation!.Roles.Count(r => r.IsKept),
            o.LogItem.SetOperation!.Channels.Count(r => !r.IsKept),
            o.LogItem.SetOperation!.Channels.Count(r => r.IsKept),
            (o.EndAtUtc - DateTime.UtcNow).TotalSeconds <= 30.0
        ));

        var result = await ContextHelper.ReadEntitiesWithPaginationAsync(mappedQuery, request.Pagination, CancellationToken);
        return ApiResult.Ok(result);
    }

    private static IQueryable<ActiveUnverify> WithSorting(IQueryable<ActiveUnverify> query, SortParameters sort)
    {
        var expressions = sort.OrderBy switch
        {
            "EndAt" => [o => o.EndAtUtc],
            _ => new Expression<Func<ActiveUnverify, object>>[] { o => o.StartAtUtc }
        };

        return query.WithSorting(expressions, sort.Descending);
    }
}

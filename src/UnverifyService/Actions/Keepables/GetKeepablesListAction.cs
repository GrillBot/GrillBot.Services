using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Services.Common.EntityFramework.Extensions;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UnverifyService.Core.Entity;
using UnverifyService.Models.Request.Keepables;
using UnverifyService.Models.Response.Keepables;

namespace UnverifyService.Actions.Keepables;

public class GetKeepablesListAction(IServiceProvider serviceProvider) : ApiAction<UnverifyContext>(serviceProvider)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<KeepablesListRequest>(0);
        var query = CreateQuery(request);
        var result = await ContextHelper.ReadEntitiesWithPaginationAsync(query, request.Pagination, CancellationToken);

        return ApiResult.Ok(result);
    }

    private IQueryable<KeepableListItem> CreateQuery(KeepablesListRequest request)
    {
        var query = DbContext.SelfUnverifyKeepables.AsNoTracking();

        if (!string.IsNullOrEmpty(request.Group))
            query = query.Where(o => EF.Functions.ILike(o.Group, $"{request.Group}%"));
        if (request.CreatedFrom is not null)
            query = query.Where(o => o.CreatedAtUtc >= request.CreatedFrom.Value);
        if (request.CreatedTo is not null)
            query = query.Where(o => o.CreatedAtUtc <= request.CreatedTo.Value);

        var sortingExpression = request.Sort.OrderBy switch
        {
            "Name" => [o => o.Name],
            "Created" => [o => o.CreatedAtUtc],
            _ => new Expression<Func<SelfUnverifyKeepable, object>>[] { o => o.Group, o => o.Name, o => o.CreatedAtUtc }
        };

        return query
            .WithSorting(sortingExpression, request.Sort.Descending)
            .Select(o => new KeepableListItem(o.Group, o.Name, o.CreatedAtUtc));
    }
}

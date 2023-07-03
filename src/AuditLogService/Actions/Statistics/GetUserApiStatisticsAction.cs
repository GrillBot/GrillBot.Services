using AuditLogService.Core.Entity;
using AuditLogService.Models.Response.Statistics;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Statistics;

public class GetUserApiStatisticsAction : ApiActionBase
{
    private AuditLogServiceContext Context { get; }

    public GetUserApiStatisticsAction(AuditLogServiceContext context)
    {
        Context = context;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var criteria = (string)Parameters[0]!;

        var data = await Filter(Context.ApiRequests.AsNoTracking(), criteria)
            .GroupBy(o => new { User = o.LogItem.UserId ?? o.Identification, o.Method, o.TemplatePath })
            .Select(o => new { Count = o.Count(), o.Key.User, o.Key.Method, o.Key.TemplatePath })
            .ToListAsync();

        var result = data
            .Select(o => new UserActionCountItem
            {
                Action = $"{o.Method} {o.TemplatePath}",
                Count = o.Count,
                UserId = o.User
            })
            .ToList();

        return ApiResult.FromSuccess(result);
    }

    private static IQueryable<ApiRequest> Filter(IQueryable<ApiRequest> query, string criteria)
    {
        query = query.Where(o => !string.IsNullOrEmpty(o.LogItem.UserId) || o.Identification != "UnknownIdentification");

        return criteria switch
        {
            "v1-private" => query.Where(o => o.ApiGroupName == "V1" && o.Identification.StartsWith("ApiV1(Private/")),
            "v1-public" => query.Where(o => o.ApiGroupName == "V1" && o.Identification.StartsWith("ApiV1(Public/")),
            "v2" => query.Where(o => o.ApiGroupName == "V2"),
            _ => throw new NotSupportedException("Unsupported criteria.")
        };
    }
}

using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Services.Common.Infrastructure.Api;
using UnverifyService.Core.Entity;

namespace UnverifyService.Actions.Keepables;

public class DeleteKeepablesAction(IServiceProvider serviceProvider) : ApiAction<UnverifyContext>(serviceProvider)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var group = GetParameter<string>(0);
        var name = GetOptionalParameter<string>(1);

        var query = DbContext.SelfUnverifyKeepables.Where(o => o.Group == group);
        if (!string.IsNullOrEmpty(name))
            query = query.Where(o => o.Name == name);

        var deletedCount = await ContextHelper.ExecuteBatchDeleteAsync(query, CancellationToken);
        return deletedCount == 0 ? ApiResult.NotFound() : ApiResult.Ok();
    }
}

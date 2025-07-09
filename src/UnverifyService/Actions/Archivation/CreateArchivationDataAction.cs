using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.Extensions.Options;
using UnverifyService.Core.Entity;
using UnverifyService.Options;

namespace UnverifyService.Actions.Archivation;

public partial class CreateArchivationDataAction(
    ICounterManager counterManager,
    UnverifyContext dbContext,
    IOptions<AppOptions> _options
) : ApiAction<UnverifyContext>(counterManager, dbContext)
{
    private DateTime ExpirationDate =>
        DateTime.UtcNow.Add(-_options.Value.Archivation.ExpirationMilestone);

    public override Task<ApiResult> ProcessAsync()
    {
        throw new NotImplementedException();
    }
}

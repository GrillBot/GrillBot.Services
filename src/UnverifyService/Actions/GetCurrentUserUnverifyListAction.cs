using GrillBot.Core.Extensions;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Services.Common.Infrastructure.Api;
using UnverifyService.Core.Entity;
using UnverifyService.Models.Request;

namespace UnverifyService.Actions;

public class GetCurrentUserUnverifyListAction(IServiceProvider serviceProvider) : ApiAction<GetActiveUnverifyListAction, UnverifyContext>(serviceProvider)
{
    public override Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<ActiveUnverifyListRequest>(0);
        var currentUserId = CurrentUser.Id.ToUlong();

        return currentUserId == 0
            ? Task.FromResult(new ApiResult(StatusCodes.Status403Forbidden, new { Message = "Current user is not authenticated." }))
            : ExecuteParentAction([request, currentUserId]);
    }
}

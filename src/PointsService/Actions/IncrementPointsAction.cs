using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PointsService.Core;
using PointsService.Core.Entity;
using PointsService.Models;
using PointsService.Models.Events;

namespace PointsService.Actions;

public class IncrementPointsAction : ApiAction
{
    public IncrementPointsAction(ICounterManager counterManager, PointsServiceContext dbContext, IRabbitMQPublisher publisher)
: base(counterManager, dbContext, publisher)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<IncrementPointsRequest>(0);

        var validationErrors = await ValidateRequestAsync(request);
        if (validationErrors is not null)
            return new(StatusCodes.Status400BadRequest, validationErrors);

        await Publisher.PublishAsync(new CreateTransactionAdminPayload(request.GuildId, request.UserId, request.Amount));
        return ApiResult.Ok();
    }

    private async Task<ValidationProblemDetails?> ValidateRequestAsync(IncrementPointsRequest request)
    {
        var modelState = new ModelStateDictionary();
        var user = await FindUserAsync(request.GuildId, request.UserId);

        if (user is null)
        {
            modelState.AddModelError(nameof(request.UserId), "UserNotFound");
        }
        else
        {
            if (!user.IsUser)
                modelState.AddModelError(nameof(request.UserId), "User is bot.");
            if (user.PointsDisabled)
                modelState.AddModelError(nameof(request.UserId), "User have disabled points.");
        }

        return modelState.IsValid ? null : new ValidationProblemDetails(modelState);
    }
}

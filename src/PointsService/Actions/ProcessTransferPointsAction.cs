using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using PointsService.Core;
using PointsService.Core.Entity;
using PointsService.Models;
using PointsService.Models.Events;

namespace PointsService.Actions;

public class ProcessTransferPointsAction : ApiAction
{
    public ProcessTransferPointsAction(ICounterManager counterManager, PointsServiceContext dbContext, IRabbitMQPublisher publisher)
        : base(counterManager, dbContext, publisher)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (TransferPointsRequest)Parameters[0]!;

        var validationErrors = await ValidateRequestAsync(request);
        if (validationErrors is not null)
            return new ApiResult(StatusCodes.Status400BadRequest, validationErrors);

        await EnqueueTransactionRequestAsync(request.GuildId, request.FromUserId, -request.Amount);
        await EnqueueTransactionRequestAsync(request.GuildId, request.ToUserId, request.Amount);

        return ApiResult.Ok();
    }

    private async Task<ValidationProblemDetails?> ValidateRequestAsync(TransferPointsRequest request)
    {
        var modelState = new ModelStateDictionary();

        await ValidateUserAsync(request.GuildId, request.FromUserId, modelState, nameof(request.FromUserId));
        await ValidateUserAsync(request.GuildId, request.ToUserId, modelState, nameof(request.ToUserId));

        var fromUserPoints = await FindUserPointsStatusAsync(request.GuildId, request.FromUserId);
        if (fromUserPoints < request.Amount)
            modelState.AddModelError(nameof(request.Amount), "NotEnoughPoints");

        return modelState.IsValid ? null : new ValidationProblemDetails(modelState);
    }

    private async Task<int> FindUserPointsStatusAsync(string guildId, string userId)
    {
        var query = DbContext.Leaderboard.AsNoTracking()
            .Where(o => o.GuildId == guildId && o.UserId == userId)
            .Select(o => o.YearBack);

        using (CreateCounter("Database"))
            return await query.FirstOrDefaultAsync();
    }

    private async Task ValidateUserAsync(string guildId, string userId, ModelStateDictionary modelState, string propertyName)
    {
        var user = await FindUserAsync(guildId, userId);
        if (user is null)
        {
            modelState.AddModelError(propertyName, "UnknownUser");
            return;
        }

        if (!user.IsUser)
            modelState.AddModelError(propertyName, "User is bot.");
        if (user.PointsDisabled)
            modelState.AddModelError(propertyName, "User have disabled points.");
    }

    private Task EnqueueTransactionRequestAsync(string guildId, string userId, int amount)
    {
        var payload = new CreateTransactionAdminPayload(guildId, userId, amount);
        return Publisher.PublishAsync(CreateTransactionAdminPayload.QueueName, payload);
    }
}

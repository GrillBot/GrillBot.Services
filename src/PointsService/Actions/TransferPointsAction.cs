using Discord;
using GrillBot.Core.Infrastructure.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PointsService.Core.Entity;
using PointsService.Core.Repository;
using PointsService.Models;

namespace PointsService.Actions;

public class TransferPointsAction : IApiAction
{
    private PointsServiceRepository Repository { get; }

    public TransferPointsAction(PointsServiceRepository repository)
    {
        Repository = repository;
    }

    public async Task<ApiResult> ProcessAsync(object?[] parameters)
    {
        var request = (TransferPointsRequest)parameters.First()!;

        var validationErrors = await ValidateRequestAsync(request);
        if (validationErrors != null)
            return new ApiResult(StatusCodes.Status400BadRequest, validationErrors);

        var transactions = new List<Transaction>
        {
            CreateTransaction(request.GuildId, request.FromUserId, -request.Amount),
            CreateTransaction(request.GuildId, request.ToUserId, request.Amount)
        };

        await Repository.AddCollectionAsync(transactions);
        await Repository.CommitAsync();
        return new ApiResult(StatusCodes.Status200OK);
    }

    private async Task<ValidationProblemDetails?> ValidateRequestAsync(TransferPointsRequest request)
    {
        var modelState = new ModelStateDictionary();

        await ValidateUserAsync(request.GuildId, request.FromUserId, modelState, nameof(request.FromUserId));
        await ValidateUserAsync(request.GuildId, request.ToUserId, modelState, nameof(request.ToUserId));

        var fromUserPoints = await Repository.Transaction.ComputePointsStatusAsync(request.GuildId, request.FromUserId, false, DateTime.UtcNow.AddYears(-1), DateTime.MaxValue);
        if (fromUserPoints < request.Amount)
            modelState.AddModelError(nameof(request.Amount), "NotEnoughPoints");

        return modelState.IsValid ? null : new ValidationProblemDetails(modelState);
    }

    private async Task ValidateUserAsync(string guildId, string userId, ModelStateDictionary modelState, string propertyName)
    {
        var user = await Repository.User.FindUserAsync(guildId, userId, true);

        if (user == null)
        {
            modelState.AddModelError(propertyName, "UnknownUser");
            return;
        }

        if (!user.IsUser)
            modelState.AddModelError(propertyName, "User is bot.");
        if (user.PointsDisabled)
            modelState.AddModelError(propertyName, "User have disabled points.");
    }

    private static Transaction CreateTransaction(string guildId, string userId, int amount)
    {
        return new Transaction
        {
            GuildId = guildId,
            MessageId = SnowflakeUtils.ToSnowflake(DateTimeOffset.UtcNow).ToString(),
            Value = amount,
            UserId = userId
        };
    }
}

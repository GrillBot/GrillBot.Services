using Discord;
using GrillBot.Core.Infrastructure.Actions;
using PointsService.Core.Entity;
using PointsService.Core.Repository;
using PointsService.Models;

namespace PointsService.Actions;

public class AdminCreateTransactionAction : ApiActionBase
{
    private PointsServiceRepository Repository { get; }

    public AdminCreateTransactionAction(PointsServiceRepository repository)
    {
        Repository = repository;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var request = (AdminTransactionRequest)Parameters[0]!;
        var user = (await Repository.User.FindUserAsync(request.GuildId, request.UserId))!;
        var messageId = SnowflakeUtils.ToSnowflake(DateTimeOffset.Now).ToString();

        if (!await CanCreateTransactionAsync(request, user, messageId))
            return new ApiResult(StatusCodes.Status406NotAcceptable);

        var transaction = new Transaction
        {
            UserId = request.UserId,
            MessageId = messageId,
            GuildId = request.GuildId,
            Value = request.Amount
        };

        user.PendingRecalculation = true;
        await Repository.AddAsync(transaction);
        await Repository.CommitAsync();

        return new ApiResult(StatusCodes.Status200OK);
    }

    private async Task<bool> CanCreateTransactionAsync(AdminTransactionRequest request, User user, string messageId)
    {
        if (!user.IsUser || user.PointsDisabled) return false;
        return !await Repository.Transaction.ExistsTransactionAsync(request.GuildId, messageId, user.Id);
    }
}

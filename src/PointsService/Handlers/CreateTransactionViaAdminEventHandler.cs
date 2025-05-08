using Discord;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using PointsService.Core.Entity;
using PointsService.Handlers.Abstractions;
using PointsService.Models.Events;

namespace PointsService.Handlers;

public class CreateTransactionViaAdminEventHandler(
    IServiceProvider serviceProvider
) : CreateTransactionBaseEventHandler<CreateTransactionAdminPayload>(serviceProvider)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(CreateTransactionAdminPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        if (!await CanCreateTransactionAsync(message))
            return RabbitConsumptionResult.Success;

        var transaction = new Transaction
        {
            GuildId = message.GuildId,
            UserId = message.UserId,
            CreatedAt = DateTime.UtcNow,
            Value = message.Amount
        };

        transaction.MessageId = SnowflakeUtils.ToSnowflake(transaction.CreatedAt).ToString();

        await CommitTransactionAsync(transaction);
        await EnqueueUserForRecalculationAsync(message.GuildId, message.UserId);
        return RabbitConsumptionResult.Success;
    }

    private async Task<bool> CanCreateTransactionAsync(CreateTransactionAdminPayload payload)
    {
        var user = await FindOrCreateUserAsync(payload.GuildId, payload.UserId);

        if (!user.IsUser)
            return await ValidationFailedAsync(payload, null, "Unable to give points to the bot.");
        if (user.PointsDisabled)
            return await ValidationFailedAsync(payload, null, "Target user have disabled points.", true);

        return true;
    }
}

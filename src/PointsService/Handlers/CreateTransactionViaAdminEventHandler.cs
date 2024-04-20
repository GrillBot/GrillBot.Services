using Discord;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using PointsService.Core.Entity;
using PointsService.Handlers.Abstractions;
using PointsService.Models.Events;

namespace PointsService.Handlers;

public class CreateTransactionViaAdminEventHandler : CreateTransactionBaseEventHandler<CreateTransactionAdminPayload>
{
    public CreateTransactionViaAdminEventHandler(ILoggerFactory loggerFactory, PointsServiceContext dbContext, ICounterManager counterManager,
        IRabbitMQPublisher publisher) : base(loggerFactory, dbContext, counterManager, publisher)
    {
    }

    protected override async Task HandleInternalAsync(CreateTransactionAdminPayload payload, Dictionary<string, string> headers)
    {
        if (!await CanCreateTransactionAsync(payload))
            return;

        var transaction = new Transaction
        {
            GuildId = payload.GuildId,
            UserId = payload.UserId,
            CreatedAt = DateTime.UtcNow,
            Value = payload.Amount
        };

        transaction.MessageId = SnowflakeUtils.ToSnowflake(transaction.CreatedAt).ToString();

        await CommitTransactionAsync(transaction);
        await EnqueueUserForRecalculationAsync(payload.GuildId, payload.UserId);
    }

    private async Task<bool> CanCreateTransactionAsync(CreateTransactionAdminPayload payload)
    {
        var user = await FindOrCreateUserAsync(payload.GuildId, payload.UserId);

        if (!user.IsUser)
            return ValidationFailed("Unable to give points to the bot.");
        if (user.PointsDisabled)
            return ValidationFailed("Target user have disabled points.", true);

        return true;
    }
}

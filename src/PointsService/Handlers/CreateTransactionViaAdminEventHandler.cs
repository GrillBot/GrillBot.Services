using Discord;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using PointsService.Core.Entity;
using PointsService.Handlers.Abstractions;
using PointsService.Models.Events;

namespace PointsService.Handlers;

public class CreateTransactionViaAdminEventHandler : CreateTransactionBaseEventHandler<CreateTransactionAdminPayload>
{
    public override string QueueName => CreateTransactionAdminPayload.QueueName;

    public CreateTransactionViaAdminEventHandler(ILoggerFactory loggerFactory, PointsServiceContext dbContext, ICounterManager counterManager,
        IRabbitMQPublisher publisher) : base(loggerFactory, dbContext, counterManager, publisher)
    {
    }

    protected override async Task HandleInternalAsync(CreateTransactionAdminPayload payload)
    {
        if (!await CanCreateTransactionAsync(payload))
            return;

        var transaction = CreateTransaction(payload);

        transaction.MessageId = SnowflakeUtils.ToSnowflake(transaction.CreatedAt).ToString();
        transaction.Value = payload.Amount;

        using (CreateCounter("Database"))
        {
            await DbContext.AddAsync(transaction);
            await DbContext.SaveChangesAsync();
        }

        await EnqueueUserForRecalculationAsync(payload);
    }

    private async Task<bool> CanCreateTransactionAsync(CreateTransactionAdminPayload payload)
    {
        var user = await FindUserAsync(payload);

        if (user is null)
            return ValidationFailed($"Unable to give points to the unknown user. ({payload.GuildId}/{payload.UserId})");
        if (!user.IsUser)
            return ValidationFailed("Unable to give points to the bot.");
        if (user.PointsDisabled)
            return ValidationFailed("Target user have disabled points.");

        return true;
    }
}

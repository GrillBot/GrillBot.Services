using GrillBot.Core.RabbitMQ;

namespace PointsService.Handlers;

public static class HandlerExtensions
{
    public static IServiceCollection AddRabbitMQ(this IServiceCollection services)
    {
        RabbitMQExtensions.AddRabbitMQ(services);

        return services
            .AddRabbitConsumerHandler<CreateTransactionViaAdminEventHandler>()
            .AddRabbitConsumerHandler<DeleteTransactionsEventHandler>()
            .AddRabbitConsumerHandler<CreateTransactionEventHandler>()
            .AddRabbitConsumerHandler<SynchronizationEventHandler>();
    }
}

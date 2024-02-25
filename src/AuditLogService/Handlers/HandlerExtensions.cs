using AuditLogService.Handlers.Recalculation;
using GrillBot.Core.RabbitMQ;

namespace AuditLogService.Handlers;

public static class HandlerExtensions
{
    public static void AddRabbitMQ(this IServiceCollection services)
    {
        RabbitMQExtensions.AddRabbitMQ(services);

        services
            .AddRabbitConsumerHandler<BulkDeleteEventHandler>()
            .AddRabbitConsumerHandler<CreateItemsEventHandler>()
            .AddRabbitConsumerHandler<RecalculationHandler>();
    }
}

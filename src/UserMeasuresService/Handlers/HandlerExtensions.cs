using GrillBot.Core.RabbitMQ;

namespace UserMeasuresService.Handlers;

public static class HandlerExtensions
{
    public static IServiceCollection AddRabbitMQHandlers(this IServiceCollection services)
    {
        return services
            .AddRabbitMQ()
            .AddRabbitConsumerHandler<UnverifyEventHandler>()
            .AddRabbitConsumerHandler<MemberWarningEventHandler>()
            .AddRabbitConsumerHandler<UnverifyModifyEventHandler>()
            .AddRabbitConsumerHandler<DeleteEventHandler>();
    }
}

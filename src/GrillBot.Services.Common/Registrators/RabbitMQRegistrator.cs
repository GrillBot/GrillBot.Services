using GrillBot.Core.RabbitMQ.Consumer;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using GrillBot.Core.RabbitMQ;
using Microsoft.Extensions.DependencyInjection;

namespace GrillBot.Services.Common.Registrators;

public static class RabbitMQRegistrator
{
    private static readonly Type _rabbitHandlerInterface = typeof(IRabbitMQHandler);

    public static void RegisterRabbitMQFromAssembly(this IServiceCollection services, IConfiguration configuration, Assembly runningAssembly)
    {
        if (!ExistsRabbitConfiguration(configuration))
            return;

        services.AddRabbitMQ();

        var types = runningAssembly.GetTypes();
        foreach (var type in types.Where(o => o.IsClass && !o.IsAbstract && o.GetInterface(_rabbitHandlerInterface.Name) is not null))
            services.AddRabbitConsumerHandler(type);
    }

    private static bool ExistsRabbitConfiguration(IConfiguration configuration)
    {
        var section = configuration.GetSection("RabbitMQ");

        return section.Exists()
            || string.IsNullOrEmpty(section["Hostname"])
            || string.IsNullOrEmpty(section["Username"])
            || string.IsNullOrEmpty(section["Password"]);
    }
}

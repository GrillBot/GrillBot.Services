using Microsoft.Extensions.Configuration;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using GrillBot.Core.RabbitMQ.V2;

namespace GrillBot.Services.Common.Registrators;

public static class RabbitMQRegistrator
{
    public static void RegisterRabbitMQFromAssembly(this IServiceCollection services, IConfiguration configuration, Assembly runningAssembly)
    {
        if (!ExistsRabbitConfiguration(configuration))
            return;

        services.AddRabbitMQ(configuration);
        services.AddRabbitConsumersFromAssembly(runningAssembly);
    }

    private static bool ExistsRabbitConfiguration(IConfiguration configuration)
    {
        var section = configuration.GetSection("RabbitMQ");

        return section.Exists()
            && !string.IsNullOrEmpty(section["Hostname"])
            && !string.IsNullOrEmpty(section["Username"])
            && !string.IsNullOrEmpty(section["Password"]);
    }
}

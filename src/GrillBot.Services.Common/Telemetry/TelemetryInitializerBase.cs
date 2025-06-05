using GrillBot.Core.Metrics.Initializer;
using GrillBot.Services.Common.EntityFramework.Helpers;
using GrillBot.Services.Common.EntityFramework.Helpers.Factory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GrillBot.Services.Common.Telemetry;

public abstract class TelemetryInitializerBase(IServiceProvider serviceProvider) : TelemetryInitializer(serviceProvider)
{
    protected ContextHelper<TContext> CreateContextHelper<TContext>(IServiceProvider provider) where TContext : DbContext
    {
        return provider
            .GetRequiredService<IContextHelperFactory<TContext>>()
            .Create($"Telemetry.Initializer.{GetType().Name}");
    }
}

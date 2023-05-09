using AuditLogService.Core.Discord;
using AuditLogService.Core.Entity;
using AuditLogService.Models.Request;

namespace AuditLogService.Processors.Request.Abstractions;

public abstract class RequestProcessorBase
{
    protected DiscordManager DiscordManager { get; }

    private IServiceProvider ServiceProvider { get; }

    protected RequestProcessorBase(IServiceProvider serviceProvider)
    {
        DiscordManager = serviceProvider.GetRequiredService<DiscordManager>();
        ServiceProvider = serviceProvider;
    }

    public abstract Task ProcessAsync(LogItem entity, LogRequest request);
}

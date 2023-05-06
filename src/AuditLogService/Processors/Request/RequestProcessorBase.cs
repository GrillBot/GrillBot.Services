using AuditLogService.Core.Discord;
using AuditLogService.Core.Entity;
using AuditLogService.Models.Request;

namespace AuditLogService.Processors.Request;

public abstract class RequestProcessorBase
{
    protected DiscordManager DiscordManager { get; }

    protected RequestProcessorBase(DiscordManager discordManager)
    {
        DiscordManager = discordManager;
    }
    
    public abstract Task ProcessAsync(LogItem entity, LogRequest request);
}

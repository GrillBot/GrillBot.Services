using Discord;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Core.Services.AuditLog.Enums;
using GrillBot.Core.Services.AuditLog.Models.Events.Create;
using PointsService.Core.Entity;
using PointsService.Models.Events;

namespace PointsService.Handlers.Abstractions;

public abstract class CreateTransactionBaseEventHandler<TPayload>(
    ILoggerFactory loggerFactory,
    PointsServiceContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher publisher
) : BasePointsEvent<TPayload>(loggerFactory, dbContext, counterManager, publisher) where TPayload : CreateTransactionBasePayload, new()
{
    protected async Task<bool> ValidationFailedAsync(TPayload payload, string? channelId, string message, bool suppressAudit = false)
    {
        Logger.LogWarning("{message}", message);

        if (!suppressAudit)
            await WriteValidationErrorToLogAsync(payload, channelId, message);

        return false;
    }

    private Task WriteValidationErrorToLogAsync(TPayload payload, string? channelId, string message)
    {
        var logRequest = new LogRequest(LogType.Warning, DateTime.UtcNow, payload.GuildId, null, channelId)
        {
            LogMessage = new LogMessageRequest
            {
                Message = message,
                Severity = LogSeverity.Warning,
                Source = GetType().Name,
                SourceAppName = "PointsService"
            }
        };

        return Publisher.PublishAsync(new CreateItemsMessage(logRequest));
    }

    protected async Task CommitTransactionAsync(Transaction transaction)
    {
        await DbContext.AddAsync(transaction);
        await ContextHelper.SaveChagesAsync();
    }
}

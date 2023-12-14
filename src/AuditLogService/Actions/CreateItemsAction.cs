using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Request.CreateItems;
using AuditLogService.Processors;
using AuditLogService.Processors.Request.Abstractions;
using GrillBot.Core.Infrastructure.Actions;
using File = AuditLogService.Core.Entity.File;

namespace AuditLogService.Actions;

public class CreateItemsAction : ApiActionBase
{
    private AuditLogServiceContext Context { get; }
    private RequestProcessorFactory RequestProcessorFactory { get; }

    public CreateItemsAction(AuditLogServiceContext context, RequestProcessorFactory requestProcessorFactory)
    {
        Context = context;
        RequestProcessorFactory = requestProcessorFactory;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var requests = (List<LogRequest>)Parameters[0]!;

        foreach (var request in requests)
        {
            var entity = await CreateEntityAsync(request);
            if (entity.CanCreate)
                await Context.AddAsync(entity);

            await Context.SaveChangesAsync();
        }

        return ApiResult.Ok();
    }

    private async Task<LogItem> CreateEntityAsync(LogRequest request)
    {
        var createdAt = request.CreatedAt ?? DateTime.UtcNow;
        var entity = new LogItem
        {
            GuildId = request.GuildId,
            Id = Guid.NewGuid(),
            Type = request.Type,
            ChannelId = request.ChannelId,
            CreatedAt = createdAt,
            UserId = request.UserId,
            CanCreate = true,
            IsPendingProcess = true,
            LogDate = DateOnly.FromDateTime(createdAt)
        };

        foreach (var file in request.Files.Select(ConvertFileRequest))
            entity.Files.Add(file);

        var requestProcessor = RequestProcessorFactory.Create(entity.Type);
        await requestProcessor.ProcessAsync(entity, request);

        await ProcessProcessingMessages(entity, requestProcessor);
        return entity;
    }

    private static File ConvertFileRequest(FileRequest request)
    {
        return new File
        {
            Extension = request.Extension,
            Id = Guid.NewGuid(),
            Filename = request.Filename,
            Size = request.Size
        };
    }

    private async Task ProcessProcessingMessages(LogItem logItem, RequestProcessorBase requestProcessor)
    {
        if (requestProcessor.ProcessingMessages.Count == 0)
            return;

        var processingMessages = requestProcessor.ProcessingMessages.ToDictionary(o => o.Key, o => o.Value);
        requestProcessor.ProcessingMessages.Clear();

        foreach (var (type, messages) in processingMessages)
        {
            var logType = type switch
            {
                Discord.LogSeverity.Warning => LogType.Warning,
                Discord.LogSeverity.Error => LogType.Error,
                Discord.LogSeverity.Info or Discord.LogSeverity.Verbose or Discord.LogSeverity.Debug => LogType.Info,
                _ => (LogType?)null
            };

            if (logType is null)
                continue; // Skip unsupported messages.

            foreach (var message in messages)
            {
                var logRequest = new LogRequest
                {
                    LogMessage = new LogMessageRequest
                    {
                        Message = message,
                        Severity = type,
                        SourceAppName = "AuditLogService",
                        Source = requestProcessor.GetType().Name
                    },
                    ChannelId = logItem.ChannelId,
                    CreatedAt = logItem.CreatedAt,
                    DiscordId = logItem.DiscordId,
                    GuildId = logItem.GuildId,
                    Type = logType.Value,
                    UserId = logItem.UserId
                };

                var entity = await CreateEntityAsync(logRequest);
                if (entity.CanCreate)
                    await Context.AddAsync(entity);
            }
        }
    }
}

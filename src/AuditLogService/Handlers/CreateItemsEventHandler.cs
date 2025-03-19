﻿using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Managers;
using AuditLogService.Models.Events.Create;
using AuditLogService.Processors;
using AuditLogService.Processors.Request.Abstractions;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;

#pragma warning disable S3604 // Member initializer values should not be redundant
namespace AuditLogService.Handlers;

public class CreateItemsEventHandler(
    ILoggerFactory loggerFactory,
    AuditLogServiceContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher publisher,
    DataRecalculationManager _dataRecalculation,
    RequestProcessorFactory _requestProcessorFactory
) : BaseEventHandlerWithDb<CreateItemsPayload, AuditLogServiceContext>(loggerFactory, dbContext, counterManager, publisher)
{
    public override string TopicName => "AuditLog";
    public override string QueueName => "CreateItems";

    private readonly CreateItemsPayload _processingInfoBatch = new();

    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(CreateItemsPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        var entities = new List<LogItem>();

        foreach (var item in message.Items)
        {
            var entity = await CreateLogItemAsync(item);
            if (!entity.CanCreate)
                continue;

            await DbContext.AddAsync(entity);
            entities.Add(entity);
        }

        await ContextHelper.SaveChagesAsync();
        await _dataRecalculation.EnqueueRecalculationAsync(entities);

        if (_processingInfoBatch.Items.Count > 0)
            await Publisher.PublishAsync("AuditLog", _processingInfoBatch, "CreateItems");
        return RabbitConsumptionResult.Success;
    }

    private async Task<LogItem> CreateLogItemAsync(LogRequest request)
    {
        var entity = new LogItem
        {
            CanCreate = true,
            ChannelId = request.ChannelId,
            CreatedAt = request.CreatedAtUtc,
            GuildId = request.GuildId,
            Id = Guid.NewGuid(),
            DiscordId = request.DiscordId,
            LogDate = DateOnly.FromDateTime(request.CreatedAtUtc),
            Type = request.Type,
            UserId = request.UserId,
            Files = request.Files.Select(CreateFile).ToHashSet()
        };

        var requestProcessor = _requestProcessorFactory.Create(request.Type);
        using (CreateCounter(requestProcessor.GetType().Name))
            await requestProcessor.ProcessAsync(entity, request);

        AddProcessingInfoToBatch(entity, requestProcessor);
        return entity;
    }

    private static Core.Entity.File CreateFile(FileRequest request)
    {
        return new Core.Entity.File
        {
            Extension = request.Extension,
            Filename = request.Filename,
            Id = Guid.NewGuid(),
            Size = request.Size
        };
    }

    private void AddProcessingInfoToBatch(LogItem entity, RequestProcessorBase processor)
    {
        if (processor.ProcessingMessages.Count == 0)
            return;

        var processingInfo = processor.ProcessingMessages.ToDictionary(o => o.Key, o => o.Value);
        processor.ProcessingMessages.Clear();

        foreach (var (type, messages) in processingInfo)
        {
            var logType = type switch
            {
                Discord.LogSeverity.Warning => LogType.Warning,
                Discord.LogSeverity.Error => LogType.Error,
                Discord.LogSeverity.Info or Discord.LogSeverity.Verbose or Discord.LogSeverity.Debug => LogType.Info,
                _ => (LogType?)null
            };

            if (logType is null)
                continue;

            foreach (var message in messages)
            {
                _processingInfoBatch.Items.Add(new LogRequest
                {
                    ChannelId = entity.ChannelId,
                    CreatedAtUtc = DateTime.UtcNow,
                    DiscordId = entity.DiscordId,
                    GuildId = entity.GuildId,
                    Type = logType.Value,
                    UserId = entity.UserId,
                    LogMessage = new LogMessageRequest
                    {
                        Message = message,
                        Severity = type,
                        Source = "AuditLogService",
                        SourceAppName = processor.GetType().Name
                    }
                });
            }
        }
    }
}

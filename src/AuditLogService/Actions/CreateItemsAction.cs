using AuditLogService.Core.Entity;
using AuditLogService.Models.Request.CreateItems;
using AuditLogService.Processors;
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
            if (!entity.CanCreate) continue;

            await Context.AddAsync(entity);
            await Context.SaveChangesAsync();
        }

        return ApiResult.FromSuccess();
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
}

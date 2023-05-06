using AuditLogService.Core.Entity;
using AuditLogService.Models.Request;
using AuditLogService.Processors.Request;
using GrillBot.Core.Infrastructure.Actions;

namespace AuditLogService.Actions;

public class CreateItemsAction : ApiActionBase
{
    private AuditLogServiceContext Context { get; }
    private IEnumerable<RequestProcessorBase> RequestProcessors { get; }

    public CreateItemsAction(AuditLogServiceContext context, IEnumerable<RequestProcessorBase> requestProcessors)
    {
        Context = context;
        RequestProcessors = requestProcessors;
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

        return new ApiResult(StatusCodes.Status200OK);
    }

    private async Task<LogItem> CreateEntityAsync(LogRequest request)
    {
        var entity = new LogItem
        {
            GuildId = request.GuildId,
            Id = Guid.NewGuid(),
            Type = request.Type,
            ChannelId = request.ChannelId,
            CreatedAt = request.CreatedAt ?? DateTime.UtcNow,
            UserId = request.UserId,
            CanCreate = true
        };

        foreach (var processor in RequestProcessors)
        {
            await processor.ProcessAsync(entity, request);

            // If someone block creation, is not required to run next processors. This entity will not be created in the database.
            if (!entity.CanCreate) break;
        }

        return entity;
    }
}

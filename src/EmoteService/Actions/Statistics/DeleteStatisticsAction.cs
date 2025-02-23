using Discord;
using EmoteService.Core.Entity;
using EmoteService.Extensions.QueryExtensions;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.Publisher;
using GrillBot.Core.Services.AuditLog.Enums;
using GrillBot.Core.Services.AuditLog.Models.Events.Create;
using GrillBot.Services.Common.Infrastructure.Api;

namespace EmoteService.Actions.Statistics;

public class DeleteStatisticsAction : ApiAction<EmoteServiceContext>
{
    private readonly IRabbitPublisher _rabbitPublisher;

    public DeleteStatisticsAction(ICounterManager counterManager, EmoteServiceContext dbContext, IRabbitPublisher rabbitPublisher) : base(counterManager, dbContext)
    {
        _rabbitPublisher = rabbitPublisher;
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var emote = Emote.Parse((string)Parameters[1]!);
        var userId = Parameters.ElementAtOrDefault(2) as string;

        var statisticsQuery = DbContext.EmoteUserStatItems
            .Where(o => o.GuildId == guildId)
            .WithEmoteQuery(emote);

        if (!string.IsNullOrEmpty(userId))
            statisticsQuery = statisticsQuery.Where(o => o.UserId == userId);

        var deletedRows = await ContextHelper.ExecuteBatchDeleteAsync(statisticsQuery);
        if (deletedRows == 0)
            return ApiResult.NotFound();

        await WriteToAuditLogAsync(emote.ToString(), deletedRows, guildId, userId);
        return ApiResult.Ok(deletedRows);
    }

    private Task WriteToAuditLogAsync(string emoteId, int deletedRows, string guildId, string? userId)
    {
        var message = $"Deleted emote statistics. Emote: {emoteId}. Deleted rows: {deletedRows}. UserId: {userId ?? "<null>"}";
        var logRequest = new LogRequest(LogType.Info, DateTime.UtcNow, guildId, CurrentUser.Id, null, null)
        {
            LogMessage = new(message, LogSeverity.Info, "EmoteService", nameof(DeleteStatisticsAction))
        };

        return _rabbitPublisher.PublishAsync(new CreateItemsPayload(logRequest));
    }
}

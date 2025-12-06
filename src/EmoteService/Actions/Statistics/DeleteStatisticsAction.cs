using AuditLog.Enums;
using AuditLog.Models.Events.Create;
using EmoteService.Core.Entity;
using EmoteService.Extensions.QueryExtensions;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Services.Common.Infrastructure.Api;

namespace EmoteService.Actions.Statistics;

public class DeleteStatisticsAction(
    ICounterManager counterManager,
    EmoteServiceContext dbContext,
    IRabbitPublisher _rabbitPublisher
) : ApiAction<EmoteServiceContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var emote = Discord.Emote.Parse((string)Parameters[1]!);
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
        var logRequest = new LogRequest(LogType.Info, DateTime.UtcNow, guildId, CurrentUser.Id, null, null)
        {
            LogMessage = new LogMessageRequest
            {
                Message = $"Deleted emote statistics. Emote: {emoteId}. Deleted rows: {deletedRows}. UserId: {userId ?? "<null>"}",
                Source = nameof(DeleteStatisticsAction),
                SourceAppName = "EmoteService",
            }
        };

        return _rabbitPublisher.PublishAsync(new CreateItemsMessage(logRequest));
    }
}

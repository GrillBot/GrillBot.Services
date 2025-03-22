using Discord;
using EmoteService.Core.Entity;
using EmoteService.Extensions.QueryExtensions;
using EmoteService.Models.Response;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Core.Services.AuditLog.Enums;
using GrillBot.Core.Services.AuditLog.Models.Events.Create;
using GrillBot.Services.Common.Infrastructure.Api;

namespace EmoteService.Actions.Statistics;

public class MergeStatisticsAction(
    ICounterManager counterManager,
    EmoteServiceContext dbContext,
    IRabbitPublisher _rabbitPublisher
) : ApiAction<EmoteServiceContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = GetParameter<string>(0);
        var sourceEmote = Emote.Parse(GetParameter<string>(1));
        var destinationEmote = Emote.Parse(GetParameter<string>(2));

        var (createdEmotesCount, deletedEmotesCount) = await ProcessMergeAsync(guildId, sourceEmote, destinationEmote);
        var modifiedRowsCount = await ContextHelper.SaveChagesAsync();

        var result = new MergeStatisticsResult
        {
            CreatedEmotesCount = createdEmotesCount,
            DeletedEmotesCount = deletedEmotesCount,
            ModifiedEmotesCount = modifiedRowsCount
        };

        await NotifyAuditLogServiceAsync(result, sourceEmote, destinationEmote, guildId);
        return ApiResult.Ok(result);
    }

    private async Task<(int createdEmotesCount, int deletedEmotesCount)> ProcessMergeAsync(string guildId, Emote sourceEmote, Emote destinationEmote)
    {
        var baseQuery = DbContext.EmoteUserStatItems.Where(o => o.GuildId == guildId);
        var sourceStatistics = await ContextHelper.ReadEntitiesAsync(baseQuery.WithEmoteQuery(sourceEmote));
        if (sourceStatistics.Count == 0)
            return (0, 0);

        var createdEmotesCount = 0;
        var deletedEmotesCount = 0;

        var destinationStatistics = await ContextHelper.ReadEntitiesAsync(baseQuery.WithEmoteQuery(destinationEmote));
        foreach (var sourceStatistic in sourceStatistics)
        {
            var destinationStatistic = destinationStatistics.Find(o => o.UserId == sourceStatistic.UserId);
            if (destinationStatistic is null)
            {
                destinationStatistic = new EmoteUserStatItem
                {
                    EmoteName = destinationEmote.Name,
                    EmoteId = destinationEmote.Id.ToString(),
                    EmoteIsAnimated = destinationEmote.Animated,
                    GuildId = sourceStatistic.GuildId,
                    UserId = sourceStatistic.UserId
                };

                await DbContext.AddAsync(destinationStatistic);
                createdEmotesCount++;
            }

            if (sourceStatistic.LastOccurence > destinationStatistic.LastOccurence)
                destinationStatistic.LastOccurence = sourceStatistic.LastOccurence;

            if (destinationStatistic.FirstOccurence == DateTime.MinValue || destinationStatistic.FirstOccurence > sourceStatistic.FirstOccurence)
                destinationStatistic.FirstOccurence = sourceStatistic.FirstOccurence;

            destinationStatistic.UseCount += sourceStatistic.UseCount;
            DbContext.Remove(sourceStatistic);
            deletedEmotesCount++;
        }

        return (createdEmotesCount, deletedEmotesCount);
    }

    private Task NotifyAuditLogServiceAsync(MergeStatisticsResult result, Emote sourceEmote, Emote destinationEmote, string guildId)
    {
        var message = $"Merged emotes {sourceEmote} into {destinationEmote}. Created: {result.CreatedEmotesCount}. Deleted: {result.DeletedEmotesCount}. Total: {result.ModifiedEmotesCount}.";
        var logRequest = new LogRequest(LogType.Info, DateTime.UtcNow, guildId, CurrentUser.Id, null, null)
        {
            LogMessage = new LogMessageRequest(message, LogSeverity.Info, "EmoteService", nameof(MergeStatisticsAction))
        };

        return _rabbitPublisher.PublishAsync(new CreateItemsMessage(logRequest));
    }

}

using AuditLog.Enums;
using AuditLog.Models.Events.Create;
using EmoteService.Core.Entity;
using EmoteService.Extensions.QueryExtensions;
using EmoteService.Models.Response;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Publisher;
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
        var sourceEmote = Discord.Emote.Parse(GetParameter<string>(1));
        var destinationEmote = Discord.Emote.Parse(GetParameter<string>(2));

        var (createdEmotesCount, deletedEmotesCount) = await ProcessMergeAsync(guildId, sourceEmote, destinationEmote);
        var modifiedRowsCount = await ContextHelper.SaveChangesAsync();

        var result = new MergeStatisticsResult
        {
            CreatedEmotesCount = createdEmotesCount,
            DeletedEmotesCount = deletedEmotesCount,
            ModifiedEmotesCount = modifiedRowsCount
        };

        await NotifyAuditLogServiceAsync(result, sourceEmote, destinationEmote, guildId);
        return ApiResult.Ok(result);
    }

    private async Task<(int createdEmotesCount, int deletedEmotesCount)> ProcessMergeAsync(string guildId, Discord.Emote sourceEmote, Discord.Emote destinationEmote)
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

    private Task NotifyAuditLogServiceAsync(MergeStatisticsResult result, Discord.Emote sourceEmote, Discord.Emote destinationEmote, string guildId)
    {
        var logRequest = new LogRequest(LogType.Info, DateTime.UtcNow, guildId, CurrentUser.Id, null, null)
        {
            LogMessage = new LogMessageRequest
            {
                SourceAppName = "EmoteService",
                Source = nameof(MergeStatisticsAction),
                Message = $"Merged emotes {sourceEmote} into {destinationEmote}. Created: {result.CreatedEmotesCount}. Deleted: {result.DeletedEmotesCount}. Total: {result.ModifiedEmotesCount}."
            }
        };

        return _rabbitPublisher.PublishAsync(new CreateItemsMessage(logRequest));
    }

}

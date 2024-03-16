using Discord;
using EmoteService.Core.Entity;
using EmoteService.Extensions.QueryExtensions;
using EmoteService.Models.Response;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EmoteService.Actions.Statistics;

public class MergeStatisticsAction : ApiAction<EmoteServiceContext>
{
    public MergeStatisticsAction(ICounterManager counterManager, EmoteServiceContext dbContext) : base(counterManager, dbContext)
    {
    }

    public override async Task<ApiResult> ProcessAsync()
    {
        var guildId = (string)Parameters[0]!;
        var sourceEmote = Emote.Parse((string)Parameters[1]!);
        var destinationEmote = Emote.Parse((string)Parameters[2]!);

        var (createdEmotesCount, deletedEmotesCount) = await ProcessMergeAsync(guildId, sourceEmote, destinationEmote);
        var modifiedRowsCount = await ContextHelper.SaveChagesAsync();

        var result = new MergeStatisticsResult
        {
            CreatedEmotesCount = createdEmotesCount,
            DeletedEmotesCount = deletedEmotesCount,
            ModifiedEmotesCount = modifiedRowsCount
        };

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
}

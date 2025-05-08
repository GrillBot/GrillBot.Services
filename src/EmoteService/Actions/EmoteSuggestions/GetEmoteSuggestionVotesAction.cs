using EmoteService.Core.Entity;
using EmoteService.Core.Entity.Suggestions;
using EmoteService.Models.Request.EmoteSuggestions;
using EmoteService.Models.Response.EmoteSuggestions;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.EntityFramework.Extensions;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;

namespace EmoteService.Actions.EmoteSuggestions;

public class GetEmoteSuggestionVotesAction(
    ICounterManager counterManager,
    EmoteServiceContext dbContext
) : ApiAction<EmoteServiceContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var suggestionId = GetParameter<Guid>(0);
        var request = GetParameter<EmoteSuggestionVoteListRequest>(1);

        var suggestionQuery = DbContext.EmoteSuggestions.AsNoTracking()
            .Where(o => o.Id == suggestionId && o.VoteSession != null);

        if (!await ContextHelper.IsAnyAsync(suggestionQuery))
            return ApiResult.NotFound();

        var query = DbContext.EmoteUserVotes.AsNoTracking()
            .Where(o => o.VoteSessionId == suggestionId);

        var dataQuery = WithFilter(query, request)
            .WithSorting([e => e.UpdatedAtUtc], request.Sort.Descending)
            .Select(o => new EmoteSuggestionVoteItem(o.UserId, o.IsApproved, o.UpdatedAtUtc));

        var result = await ContextHelper.ReadEntitiesWithPaginationAsync(dataQuery, request.Pagination);
        return ApiResult.Ok(result);
    }

    private static IQueryable<EmoteUserVote> WithFilter(IQueryable<EmoteUserVote> query, EmoteSuggestionVoteListRequest request)
    {
        if (request.UserId is not null)
            query = query.Where(o => o.UserId == request.UserId);

        return query;
    }
}

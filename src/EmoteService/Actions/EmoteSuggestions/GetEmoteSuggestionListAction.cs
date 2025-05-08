using EmoteService.Core.Entity;
using EmoteService.Core.Entity.Suggestions;
using EmoteService.Models.Request.EmoteSuggestions;
using EmoteService.Models.Response.EmoteSuggestions;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.Models;
using GrillBot.Services.Common.EntityFramework.Extensions;
using GrillBot.Services.Common.Infrastructure.Api;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EmoteService.Actions.EmoteSuggestions;

public class GetEmoteSuggestionListAction(
    ICounterManager counterManager,
    EmoteServiceContext dbContext
) : ApiAction<EmoteServiceContext>(counterManager, dbContext)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var request = GetParameter<EmoteSuggestionsListRequest>(0);

        var query = DbContext.EmoteSuggestions
            .Include(o => o.VoteSession).ThenInclude(o => o!.UserVotes)
            .AsNoTracking();

        query = WithFilter(query, request);
        query = WithSorting(query, request.Sort);

        var dataQuery = WithProjection(query);
        var result = await ContextHelper.ReadEntitiesWithPaginationAsync(dataQuery, request.Pagination);

        return ApiResult.Ok(result);
    }

    private static IQueryable<EmoteSuggestion> WithFilter(IQueryable<EmoteSuggestion> query, EmoteSuggestionsListRequest request)
    {
        if (request.GuildId is not null)
            query = query.Where(o => o.GuildId == request.GuildId);
        if (request.FromUserId is not null)
            query = query.Where(o => o.FromUserId == request.FromUserId);
        if (request.SuggestedFrom is not null)
            query = query.Where(o => o.SuggestedAtUtc >= request.SuggestedFrom);
        if (request.SuggestedTo is not null)
            query = query.Where(o => o.SuggestedAtUtc <= request.SuggestedTo);
        if (request.NameContains is not null)
            query = query.Where(o => EF.Functions.ILike(o.Name, $"%{request.NameContains}%"));
        if (request.ApprovalState is not null)
            query = query.Where(o => o.ApprovedForVote == request.ApprovalState.Value);

        return query;
    }

    private static IQueryable<EmoteSuggestion> WithSorting(IQueryable<EmoteSuggestion> query, SortParameters sortParams)
    {
        var expressions = sortParams.OrderBy?.ToLower() switch
        {
            "name" => [entity => entity.Name],
            _ => new Expression<Func<EmoteSuggestion, object>>[] { entity => entity.SuggestedAtUtc }
        };

        return query.WithSorting(expressions, sortParams.Descending);
    }

    private static IQueryable<EmoteSuggestionItem> WithProjection(IQueryable<EmoteSuggestion> query)
    {
        return query.Select(o => new EmoteSuggestionItem(
            o.Id,
            o.FromUserId,
            o.Name,
            o.SuggestedAtUtc,
            o.GuildId,
            o.SuggestionMessageId,
            o.ApprovedForVote,
            o.ApprovalByUserId,
            o.ApprovalSetAtUtc,
            o.ReasonForAdd,
            o.VoteSession!.VoteStartedAtUtc,
            o.VoteSession!.ExpectedVoteEndAtUtc,
            o.VoteSession!.KilledAtUtc,
            o.VoteSession == null ? null : o.VoteSession!.UserVotes.Count(o => o.IsApproved),
            o.VoteSession == null ? null : o.VoteSession!.UserVotes.Count(o => !o.IsApproved)
        ));
    }
}

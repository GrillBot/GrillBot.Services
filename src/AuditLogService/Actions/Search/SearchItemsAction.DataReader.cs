using AuditLogService.Core.Entity;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Request.Search;
using Discord;
using GrillBot.Core.Models.Pagination;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Actions.Search;

public partial class SearchItemsAction
{
    private async Task<List<Guid>?> SearchIdsFromAdvancedFilterAsync(SearchRequest request)
    {
        if ((request.Ids is not null && request.Ids.Count > 0) || !request.IsAnyAdvancedFilterSet())
            return null; // Ignore advanced filters if IDs was specified explicitly.

        var result = new List<Guid>();
        var advancedSearch = request.AdvancedSearch!;

        if (request.IsAdvancedFilterSet(LogType.Info) || request.IsAdvancedFilterSet(LogType.Warning) || request.IsAdvancedFilterSet(LogType.Error))
        {
            var query = CreateCommonFilterForAdvancedSearch(Context.LogMessages, request);
            TextSearchRequest? searchReq = null;

            if (request.IsAdvancedFilterSet(LogType.Info))
            {
                searchReq = advancedSearch.Info;
                query = query.Where(o => o.Severity == LogSeverity.Info);
            }
            else if (request.IsAdvancedFilterSet(LogType.Warning))
            {
                searchReq = advancedSearch.Warning;
                query = query.Where(o => o.Severity == LogSeverity.Warning);
            }
            else if (request.IsAdvancedFilterSet(LogType.Error))
            {
                searchReq = advancedSearch.Error;
                query = query.Where(o => o.Severity == LogSeverity.Error);
            }

            if (searchReq is not null)
            {
                if (!string.IsNullOrEmpty(searchReq.Source))
                    query = query.Where(o => EF.Functions.ILike(o.Source, searchReq.Source));
                if (!string.IsNullOrEmpty(searchReq.SourceAppName))
                    query = query.Where(o => EF.Functions.ILike(o.SourceAppName, searchReq.SourceAppName));
                if (!string.IsNullOrEmpty(searchReq.Text))
                    query = query.Where(o => EF.Functions.ILike(o.Message, $"%{searchReq.Text}%"));

                result.AddRange(await SelectIdsAsync(query));
            }
        }

        if (request.IsAdvancedFilterSet(LogType.InteractionCommand))
        {
            var baseQuery = CreateCommonFilterForAdvancedSearch(Context.InteractionCommands, request);
            var searchReq = advancedSearch.Interaction!;

            if (!string.IsNullOrEmpty(searchReq.ActionName))
            {
                baseQuery = baseQuery.Where(o =>
                    EF.Functions.ILike(o.Name, $"%{searchReq.ActionName}%") || EF.Functions.ILike(o.ModuleName, $"%{searchReq.ActionName}%") ||
                    EF.Functions.ILike(o.MethodName, $"%{searchReq.ActionName}%"));
            }

            if (searchReq.Success is not null)
                baseQuery = baseQuery.Where(o => o.IsSuccess == searchReq.Success);
            if (searchReq.DurationFrom is not null)
                baseQuery = baseQuery.Where(o => o.Duration >= searchReq.DurationFrom.Value);
            if (searchReq.DurationTo is not null)
                baseQuery = baseQuery.Where(o => o.Duration <= searchReq.DurationTo.Value);

            result.AddRange(await SelectIdsAsync(baseQuery));
        }

        if (request.IsAdvancedFilterSet(LogType.JobCompleted))
        {
            var baseQuery = CreateCommonFilterForAdvancedSearch(Context.JobExecutions, request);
            var searchReq = advancedSearch.Job!;

            if (!string.IsNullOrEmpty(searchReq.ActionName))
                baseQuery = baseQuery.Where(o => o.JobName.Contains(searchReq.ActionName));
            if (searchReq.Success is not null)
                baseQuery = baseQuery.Where(o => o.WasError != searchReq.Success);
            if (searchReq.DurationFrom is not null)
                baseQuery = baseQuery.Where(o => o.Duration >= searchReq.DurationFrom.Value);
            if (searchReq.DurationTo is not null)
                baseQuery = baseQuery.Where(o => o.Duration <= searchReq.DurationTo.Value);

            result.AddRange(await SelectIdsAsync(baseQuery));
        }

        if (request.IsAdvancedFilterSet(LogType.Api))
        {
            var baseQuery = CreateCommonFilterForAdvancedSearch(Context.ApiRequests, request);
            var searchReq = advancedSearch.Api!;

            if (!string.IsNullOrEmpty(searchReq.ControllerName))
                baseQuery = baseQuery.Where(o => o.ControllerName.Contains(searchReq.ControllerName));
            if (!string.IsNullOrEmpty(searchReq.ActionName))
                baseQuery = baseQuery.Where(o => o.ActionName.Contains(searchReq.ActionName));
            if (!string.IsNullOrEmpty(searchReq.PathTemplate))
                baseQuery = baseQuery.Where(o => o.TemplatePath.Contains(searchReq.PathTemplate));
            if (searchReq.DurationFrom is not null)
                baseQuery = baseQuery.Where(o => o.Duration >= searchReq.DurationFrom.Value);
            if (searchReq.DurationTo is not null)
                baseQuery = baseQuery.Where(o => o.Duration <= searchReq.DurationTo.Value);
            if (!string.IsNullOrEmpty(searchReq.Method))
                baseQuery = baseQuery.Where(o => o.Method == searchReq.Method);
            if (!string.IsNullOrEmpty(searchReq.ApiGroupName))
                baseQuery = baseQuery.Where(o => o.ApiGroupName == searchReq.ApiGroupName);
            if (!string.IsNullOrEmpty(searchReq.Identification))
                baseQuery = baseQuery.Where(o => EF.Functions.ILike(o.Identification, $"%{searchReq.Identification}%"));

            result.AddRange(await SelectIdsAsync(baseQuery));
        }

        if (request.IsAdvancedFilterSet(LogType.OverwriteCreated))
        {
            var query = CreateCommonFilterForAdvancedSearch(Context.OverwriteCreatedItems, request)
                .Where(o => o.OverwriteInfo.TargetId == advancedSearch.OverwriteCreated!.UserId);

            result.AddRange(await SelectIdsAsync(query));
        }

        if (request.IsAdvancedFilterSet(LogType.OverwriteDeleted))
        {
            var query = CreateCommonFilterForAdvancedSearch(Context.OverwriteDeletedItems, request)
                .Where(o => o.OverwriteInfo.TargetId == advancedSearch.OverwriteDeleted!.UserId);

            result.AddRange(await SelectIdsAsync(query));
        }

        if (request.IsAdvancedFilterSet(LogType.OverwriteUpdated))
        {
            var query = CreateCommonFilterForAdvancedSearch(Context.OverwriteUpdatedItems, request)
                .Where(o => o.Before.TargetId == advancedSearch.OverwriteUpdated!.UserId || o.After.TargetId == advancedSearch.OverwriteUpdated!.UserId);

            result.AddRange(await SelectIdsAsync(query));
        }

        if (request.IsAdvancedFilterSet(LogType.MemberRoleUpdated))
        {
            result.AddRange(
                await Context.MemberRoleUpdatedItems.AsNoTracking()
                    .Where(o => o.UserId == advancedSearch.MemberRolesUpdated!.UserId)
                    .Select(o => o.LogItemId)
                    .ToListAsync()
            );
        }

        if (request.IsAdvancedFilterSet(LogType.MemberUpdated))
        {
            var query = CreateCommonFilterForAdvancedSearch(Context.MemberUpdatedItems, request)
                .Where(o => o.Before.UserId == advancedSearch.MemberUpdated!.UserId || o.After.UserId == advancedSearch.MemberUpdated!.UserId);

            result.AddRange(await SelectIdsAsync(query));
        }

        if (request.IsAdvancedFilterSet(LogType.MessageDeleted))
        {
            var baseQuery = CreateCommonFilterForAdvancedSearch(Context.MessageDeletedItems, request);
            var searchReq = advancedSearch.MessageDeleted!;

            if (searchReq.ContainsEmbed is not null)
                baseQuery = searchReq.ContainsEmbed.Value ? baseQuery.Where(o => o.Embeds.Count > 0) : baseQuery.Where(o => o.Embeds.Count == 0);
            if (!string.IsNullOrEmpty(searchReq.ContentContains))
                baseQuery = baseQuery.Where(o => !string.IsNullOrEmpty(o.Content) && o.Content.Contains(searchReq.ContentContains));
            if (!string.IsNullOrEmpty(searchReq.AuthorId))
                baseQuery = baseQuery.Where(o => o.AuthorId == searchReq.AuthorId);

            result.AddRange(await SelectIdsAsync(baseQuery));
        }

        return result.Distinct().ToList();
    }

    private static IQueryable<TChildEntity> CreateCommonFilterForAdvancedSearch<TChildEntity>(IQueryable<TChildEntity> baseQuery, SearchRequest request) where TChildEntity : ChildEntityBase
    {
        var query = baseQuery.AsNoTracking();

        if (!string.IsNullOrEmpty(request.GuildId))
            query = query.Where(o => o.LogItem.GuildId == request.GuildId);
        if (request.UserIds.Count > 0)
            query = query.Where(o => !string.IsNullOrEmpty(o.LogItem.UserId) && request.UserIds.Contains(o.LogItem.UserId));
        if (!string.IsNullOrEmpty(request.ChannelId))
            query = query.Where(o => o.LogItem.ChannelId == request.ChannelId);
        if (request.CreatedFrom is not null)
            query = query.Where(o => o.LogItem.CreatedAt >= request.CreatedFrom.Value);
        if (request.CreatedTo is not null)
            query = query.Where(o => o.LogItem.CreatedAt <= request.CreatedTo.Value);
        if (request.OnlyWithFiles)
            query = query.Where(o => o.LogItem.Files.Count > 0);

        return query;
    }

    private static async Task<List<Guid>> SelectIdsAsync<TChildEntity>(IQueryable<TChildEntity> query) where TChildEntity : ChildEntityBase
        => await query.Select(o => o.LogItemId).ToListAsync();

    private async Task<PaginatedResponse<LogItem>> ReadLogHeaders(SearchRequest request)
    {
        var query = Context.LogItems.Include(o => o.Files).AsNoTracking()
            .Where(o => !o.IsDeleted);

        if (request.ShowTypes.Count > 0)
            query = query.Where(o => request.ShowTypes.Contains(o.Type));
        else if (request.IgnoreTypes.Count > 0)
            query = query.Where(o => !request.IgnoreTypes.Contains(o.Type));

        if (!string.IsNullOrEmpty(request.GuildId))
            query = query.Where(o => o.GuildId == request.GuildId);
        if (request.UserIds.Count > 0)
            query = query.Where(o => !string.IsNullOrEmpty(o.UserId) && request.UserIds.Contains(o.UserId));
        if (!string.IsNullOrEmpty(request.ChannelId))
            query = query.Where(o => o.ChannelId == request.ChannelId);
        if (request.CreatedFrom is not null)
            query = query.Where(o => o.CreatedAt >= request.CreatedFrom.Value);
        if (request.CreatedTo is not null)
            query = query.Where(o => o.CreatedAt <= request.CreatedTo.Value);
        if (request.OnlyWithFiles)
            query = query.Where(o => o.Files.Count > 0);
        if (request.Ids is not null)
            query = query.Where(o => request.Ids.Contains(o.Id));

        query = request.Sort.Descending ? query.OrderByDescending(o => o.CreatedAt) : query.OrderBy(o => o.CreatedAt);
        return await PaginatedResponse<LogItem>.CreateWithEntityAsync(query, request.Pagination);
    }
}

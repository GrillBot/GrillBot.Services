﻿using Discord;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using Microsoft.EntityFrameworkCore;
using PointsService.Core;
using PointsService.Core.Entity;
using PointsService.Models;
using System.Diagnostics;

namespace PointsService.Actions.Merge;

public abstract class MergeTransactionsBaseAction(
    ICounterManager counterManager,
    PointsServiceContext dbContext,
    IRabbitPublisher publisher
) : ApiAction(counterManager, dbContext, publisher)
{
    protected abstract Task InitializeAsync();
    protected abstract Task<bool> CanProcessMergeAsync();
    protected abstract Task<List<Transaction>> GetTransactionsForMergeAsync();
    protected abstract List<Transaction> ProcessMergeInternal(List<Transaction> transactions);
    public abstract Task<int> CountAsync();

    public override async Task<ApiResult> ProcessAsync()
    {
        await InitializeAsync();

        if (!await CanProcessMergeAsync())
            return new ApiResult(StatusCodes.Status204NoContent);

        var stopwatch = Stopwatch.StartNew();
        var transactions = await GetTransactionsForMergeAsync();
        var mergedTransactions = ProcessMerge(transactions);
        var dailyStatsToDelete = await GetDailyStatsForDeleteAsync(transactions, mergedTransactions);

        DbContext.RemoveRange(transactions);
        DbContext.RemoveRange(dailyStatsToDelete);
        await DbContext.AddRangeAsync(mergedTransactions);
        await ContextHelper.SaveChangesAsync();
        await EnqueueUserRecalculationsAsync(mergedTransactions);

        stopwatch.Stop();
        var result = new MergeResult
        {
            Duration = stopwatch.Elapsed.ToString("c"),
            ExpiredCount = transactions.Count,
            GuildCount = mergedTransactions.DistinctBy(o => o.GuildId).Count(),
            MergedCount = mergedTransactions.Count,
            TotalPoints = mergedTransactions.Sum(o => o.Value),
            UserCount = mergedTransactions.DistinctBy(o => o.UserId).Count(),
            DeletedDailyStatsCount = dailyStatsToDelete.Count
        };

        return ApiResult.Ok(result);
    }

    private List<Transaction> ProcessMerge(List<Transaction> transactions)
    {
        using (CreateCounter("ProcessMerge"))
            return ProcessMergeInternal(transactions);
    }

    private async Task EnqueueUserRecalculationsAsync(List<Transaction> mergedTransactions)
    {
        var group = mergedTransactions
            .GroupBy(o => new { o.GuildId, o.UserId })
            .Select(o => o.First());

        foreach (var user in group)
            await EnqueueUserForRecalculationAsync(user.GuildId, user.UserId);
    }

    private async Task<List<DailyStat>> GetDailyStatsForDeleteAsync(List<Transaction> oldTransactions, List<Transaction> mergedTransactions)
    {
        var from = oldTransactions.Min(o => o.CreatedAt.Date);
        var to = oldTransactions.Max(o => o.CreatedAt.Date).AddDays(1);
        var fromDate = new DateOnly(from.Year, from.Month, from.Day);
        var toDate = new DateOnly(to.Year, to.Month, to.Day);
        var userIds = mergedTransactions.Select(o => o.UserId).Distinct().ToList();
        var guildIds = mergedTransactions.Select(o => o.GuildId).Distinct().ToList();

        var query = DbContext.DailyStats.Where(o => o.Date >= fromDate && o.Date <= toDate && userIds.Contains(o.UserId) && guildIds.Contains(o.GuildId));
        return await ContextHelper.ReadEntitiesAsync(query);
    }

    protected static Transaction CloneTransaction(Transaction transaction)
    {
        return new Transaction
        {
            GuildId = transaction.GuildId,
            UserId = transaction.UserId,
            MessageId = SnowflakeUtils.ToSnowflake(DateTimeOffset.Now).ToString(),
            CreatedAt = DateTime.MaxValue,
            ReactionId = "",

            MergeRangeFrom = transaction.MergeRangeFrom,
            MergeRangeTo = transaction.MergeRangeTo
        };
    }

    protected static void MergeTransactionProperties(Transaction merged, Transaction old)
    {
        MergeTransactionDateProperties(merged, old);

        merged.Value += old.Value;
        merged.CreatedAt = merged.MergeRangeFrom.GetValueOrDefault();
        merged.MergedCount += Math.Max(old.MergedCount, 1);
    }

    private static void MergeTransactionDateProperties(Transaction merged, Transaction old)
    {
        if (old.MergedCount > 0)
        {
            if (old.MergeRangeFrom!.Value <= (merged.MergeRangeFrom ?? DateTime.MaxValue))
                merged.MergeRangeFrom = old.MergeRangeFrom!.Value;
            if (old.MergeRangeTo!.Value >= (merged.MergeRangeTo ?? DateTime.MinValue))
                merged.MergeRangeTo = old.MergeRangeTo!.Value;
        }
        else
        {
            if (old.CreatedAt <= (merged.MergeRangeFrom ?? DateTime.MaxValue))
                merged.MergeRangeFrom = old.CreatedAt;
            if (old.CreatedAt >= (merged.MergeRangeTo ?? DateTime.MinValue))
                merged.MergeRangeTo = old.CreatedAt;
        }
    }
}

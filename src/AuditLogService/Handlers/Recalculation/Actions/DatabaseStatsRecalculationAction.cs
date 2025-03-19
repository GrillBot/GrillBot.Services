using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Enums;
using AuditLogService.Models.Events.Recalculation;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Handlers.Recalculation.Actions;

public class DatabaseStatsRecalculationAction(IServiceProvider serviceProvider) : RecalculationActionBase(serviceProvider)
{
    public override async Task ProcessAsync(RecalculationPayload payload)
    {
        await UpdateStatisticsRecord<LogItem>(nameof(DbContext.LogItems));

        switch (payload.Type)
        {
            case LogType.Info or LogType.Warning or LogType.Error:
                await UpdateStatisticsRecord<LogMessage>(nameof(DbContext.LogMessages));
                break;
            case LogType.ChannelCreated:
                await UpdateStatisticsRecord<ChannelCreated>(nameof(DbContext.ChannelCreatedItems));
                await UpdateStatisticsRecord<ChannelInfo>(nameof(DbContext.ChannelInfoItems));
                break;
            case LogType.ChannelDeleted:
                await UpdateStatisticsRecord<ChannelDeleted>(nameof(DbContext.ChannelDeletedItems));
                await UpdateStatisticsRecord<ChannelInfo>(nameof(DbContext.ChannelInfoItems));
                break;
            case LogType.ChannelUpdated:
                await UpdateStatisticsRecord<ChannelUpdated>(nameof(DbContext.ChannelUpdatedItems));
                await UpdateStatisticsRecord<ChannelInfo>(nameof(DbContext.ChannelInfoItems));
                break;
            case LogType.EmoteDeleted:
                await UpdateStatisticsRecord<DeletedEmote>(nameof(DbContext.DeletedEmotes));
                break;
            case LogType.OverwriteCreated:
                await UpdateStatisticsRecord<OverwriteCreated>(nameof(DbContext.OverwriteCreatedItems));
                await UpdateStatisticsRecord<OverwriteInfo>(nameof(DbContext.OverwriteInfoItems));
                break;
            case LogType.OverwriteDeleted:
                await UpdateStatisticsRecord<OverwriteDeleted>(nameof(DbContext.OverwriteDeletedItems));
                await UpdateStatisticsRecord<OverwriteInfo>(nameof(DbContext.OverwriteInfoItems));
                break;
            case LogType.OverwriteUpdated:
                await UpdateStatisticsRecord<OverwriteUpdated>(nameof(DbContext.OverwriteUpdatedItems));
                await UpdateStatisticsRecord<OverwriteInfo>(nameof(DbContext.OverwriteInfoItems));
                break;
            case LogType.Unban:
                await UpdateStatisticsRecord<Unban>(nameof(DbContext.Unbans));
                break;
            case LogType.MemberUpdated:
                await UpdateStatisticsRecord<MemberUpdated>(nameof(DbContext.MemberUpdatedItems));
                await UpdateStatisticsRecord<MemberInfo>(nameof(DbContext.MemberInfos));
                break;
            case LogType.MemberRoleUpdated:
                await UpdateStatisticsRecord<MemberRoleUpdated>(nameof(DbContext.MemberRoleUpdatedItems));
                break;
            case LogType.GuildUpdated:
                await UpdateStatisticsRecord<GuildUpdated>(nameof(DbContext.GuildUpdatedItems));
                await UpdateStatisticsRecord<GuildInfo>(nameof(DbContext.GuildInfoItems));
                break;
            case LogType.UserLeft:
                await UpdateStatisticsRecord<UserLeft>(nameof(DbContext.UserLeftItems));
                break;
            case LogType.UserJoined:
                await UpdateStatisticsRecord<UserJoined>(nameof(DbContext.UserJoinedItems));
                break;
            case LogType.MessageEdited:
                await UpdateStatisticsRecord<MessageEdited>(nameof(DbContext.MessageEditedItems));
                break;
            case LogType.MessageDeleted:
                await UpdateStatisticsRecord<Core.Entity.File>(nameof(DbContext.Files));
                await UpdateStatisticsRecord<MessageDeleted>(nameof(DbContext.MessageDeletedItems));
                await UpdateStatisticsRecord<EmbedInfo>(nameof(DbContext.EmbedInfoItems));
                await UpdateStatisticsRecord<EmbedField>(nameof(DbContext.EmbedFields));
                break;
            case LogType.InteractionCommand:
                await UpdateStatisticsRecord<InteractionCommand>(nameof(DbContext.InteractionCommands));
                break;
            case LogType.ThreadDeleted:
                await UpdateStatisticsRecord<ThreadDeleted>(nameof(DbContext.ThreadDeletedItems));
                await UpdateStatisticsRecord<ThreadInfo>(nameof(DbContext.ThreadInfoItems));
                break;
            case LogType.JobCompleted:
                await UpdateStatisticsRecord<JobExecution>(nameof(DbContext.JobExecutions));
                break;
            case LogType.Api:
                await UpdateStatisticsRecord<ApiRequest>(nameof(DbContext.ApiRequests));
                break;
            case LogType.ThreadUpdated:
                await UpdateStatisticsRecord<ThreadUpdated>(nameof(DbContext.ThreadUpdatedItems));
                await UpdateStatisticsRecord<ThreadInfo>(nameof(DbContext.ThreadInfoItems));
                break;
            case LogType.RoleDeleted:
                await UpdateStatisticsRecord<RoleDeleted>(nameof(DbContext.RoleDeleted));
                await UpdateStatisticsRecord<RoleInfo>(nameof(DbContext.RoleInfos));
                break;
        }

        await StatisticsContext.SaveChangesAsync();

        switch (payload.Type)
        {
            case LogType.Api:
                await UpdateStatisticsRecord<ApiRequestStat>($"Statistics.{nameof(StatisticsContext.RequestStats)}", true);
                await UpdateStatisticsRecord<DailyAvgTimes>($"Statistics.{nameof(StatisticsContext.DailyAvgTimes)}", true);
                await UpdateStatisticsRecord<ApiUserActionStatistic>($"Statistics.{nameof(StatisticsContext.ApiUserActionStatistics)}", true);
                break;
            case LogType.JobCompleted:
                await UpdateStatisticsRecord<DailyAvgTimes>($"Statistics.{nameof(StatisticsContext.DailyAvgTimes)}", true);
                await UpdateStatisticsRecord<JobInfo>($"Statistics.{nameof(StatisticsContext.JobInfos)}", true);
                break;
            case LogType.InteractionCommand:
                await UpdateStatisticsRecord<DailyAvgTimes>($"Statistics.{nameof(StatisticsContext.DailyAvgTimes)}", true);
                await UpdateStatisticsRecord<InteractionUserActionStatistic>($"Statistics.{nameof(StatisticsContext.InteractionUserActionStatistics)}", true);
                await UpdateStatisticsRecord<InteractionStatistic>($"Statistics.{nameof(StatisticsContext.InteractionStatistics)}", true);
                break;
        }

        await StatisticsContext.SaveChangesAsync();

        await UpdateStatisticsRecord<DatabaseStatistic>($"Statistics.{nameof(StatisticsContext.DatabaseStatistics)}", true);
        await StatisticsContext.SaveChangesAsync();
    }

    private async Task UpdateStatisticsRecord<TEntity>(string tableName, bool useStatisticsContext = false) where TEntity : class
    {
        var stats = await GetOrCreateStatEntity<DatabaseStatistic>(o => o.TableName == tableName, tableName);
        DbContext context = useStatisticsContext ? StatisticsContext : DbContext;

        stats.RecordsCount = await context.Set<TEntity>().AsNoTracking().CountAsync();
    }
}

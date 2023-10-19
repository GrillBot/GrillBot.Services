using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using AuditLogService.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.BackgroundServices.Actions;

public class ComputeDatabaseStatisticsAction : PostProcessActionBase
{
    public ComputeDatabaseStatisticsAction(AuditLogServiceContext context, AuditLogStatisticsContext statisticsContext) : base(context, statisticsContext)
    {
    }

    public override bool CanProcess(LogItem logItem) => true;

    public override async Task ProcessAsync(LogItem logItem)
    {
        await UpdateStatisticsRecord<LogItem>(nameof(Context.LogItems));

        switch (logItem.Type)
        {
            case LogType.Info or LogType.Warning or LogType.Error:
                await UpdateStatisticsRecord<LogMessage>(nameof(Context.LogMessages));
                break;
            case LogType.ChannelCreated:
                await UpdateStatisticsRecord<ChannelCreated>(nameof(Context.ChannelCreatedItems));
                await UpdateStatisticsRecord<ChannelInfo>(nameof(Context.ChannelInfoItems));
                break;
            case LogType.ChannelDeleted:
                await UpdateStatisticsRecord<ChannelDeleted>(nameof(Context.ChannelDeletedItems));
                await UpdateStatisticsRecord<ChannelInfo>(nameof(Context.ChannelInfoItems));
                break;
            case LogType.ChannelUpdated:
                await UpdateStatisticsRecord<ChannelUpdated>(nameof(Context.ChannelUpdatedItems));
                await UpdateStatisticsRecord<ChannelInfo>(nameof(Context.ChannelInfoItems));
                break;
            case LogType.EmoteDeleted:
                await UpdateStatisticsRecord<DeletedEmote>(nameof(Context.DeletedEmotes));
                break;
            case LogType.OverwriteCreated:
                await UpdateStatisticsRecord<OverwriteCreated>(nameof(Context.OverwriteCreatedItems));
                await UpdateStatisticsRecord<OverwriteInfo>(nameof(Context.OverwriteInfoItems));
                break;
            case LogType.OverwriteDeleted:
                await UpdateStatisticsRecord<OverwriteDeleted>(nameof(Context.OverwriteDeletedItems));
                await UpdateStatisticsRecord<OverwriteInfo>(nameof(Context.OverwriteInfoItems));
                break;
            case LogType.OverwriteUpdated:
                await UpdateStatisticsRecord<OverwriteUpdated>(nameof(Context.OverwriteUpdatedItems));
                await UpdateStatisticsRecord<OverwriteInfo>(nameof(Context.OverwriteInfoItems));
                break;
            case LogType.Unban:
                await UpdateStatisticsRecord<Unban>(nameof(Context.Unbans));
                break;
            case LogType.MemberUpdated:
                await UpdateStatisticsRecord<MemberUpdated>(nameof(Context.MemberUpdatedItems));
                await UpdateStatisticsRecord<MemberInfo>(nameof(Context.MemberInfos));
                break;
            case LogType.MemberRoleUpdated:
                await UpdateStatisticsRecord<MemberRoleUpdated>(nameof(Context.MemberRoleUpdatedItems));
                break;
            case LogType.GuildUpdated:
                await UpdateStatisticsRecord<GuildUpdated>(nameof(Context.GuildUpdatedItems));
                await UpdateStatisticsRecord<GuildInfo>(nameof(Context.GuildInfoItems));
                break;
            case LogType.UserLeft:
                await UpdateStatisticsRecord<UserLeft>(nameof(Context.UserLeftItems));
                break;
            case LogType.UserJoined:
                await UpdateStatisticsRecord<UserJoined>(nameof(Context.UserJoinedItems));
                break;
            case LogType.MessageEdited:
                await UpdateStatisticsRecord<MessageEdited>(nameof(Context.MessageEditedItems));
                break;
            case LogType.MessageDeleted:
                await UpdateStatisticsRecord<Core.Entity.File>(nameof(Context.Files));
                await UpdateStatisticsRecord<MessageDeleted>(nameof(Context.MessageDeletedItems));
                await UpdateStatisticsRecord<EmbedInfo>(nameof(Context.EmbedInfoItems));
                await UpdateStatisticsRecord<EmbedField>(nameof(Context.EmbedFields));
                break;
            case LogType.InteractionCommand:
                await UpdateStatisticsRecord<InteractionCommand>(nameof(Context.InteractionCommands));
                break;
            case LogType.ThreadDeleted:
                await UpdateStatisticsRecord<ThreadDeleted>(nameof(Context.ThreadDeletedItems));
                await UpdateStatisticsRecord<ThreadInfo>(nameof(Context.ThreadInfoItems));
                break;
            case LogType.JobCompleted:
                await UpdateStatisticsRecord<JobExecution>(nameof(Context.JobExecutions));
                break;
            case LogType.Api:
                await UpdateStatisticsRecord<ApiRequest>(nameof(Context.ApiRequests));
                break;
            case LogType.ThreadUpdated:
                await UpdateStatisticsRecord<ThreadUpdated>(nameof(Context.ThreadUpdatedItems));
                await UpdateStatisticsRecord<ThreadInfo>(nameof(Context.ThreadInfoItems));
                break;
            case LogType.RoleDeleted:
                await UpdateStatisticsRecord<RoleDeleted>(nameof(Context.RoleDeleted));
                await UpdateStatisticsRecord<RoleInfo>(nameof(Context.RoleInfos));
                break;
        }

        await StatisticsContext.SaveChangesAsync();

        await UpdateStatisticsRecord<AuditLogTypeStatistic>($"Statistics.{nameof(StatisticsContext.TypeStatistics)}", true);
        await UpdateStatisticsRecord<AuditLogDateStatistic>($"Statistics.{nameof(StatisticsContext.DateStatistics)}", true);

        switch (logItem.Type)
        {
            case LogType.Api:
                await UpdateStatisticsRecord<ApiDateCountStatistic>($"Statistics.{nameof(StatisticsContext.DateCountStatistics)}", true);
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
                await UpdateStatisticsRecord<InteractionDateCountStatistic>($"Statistics.{nameof(StatisticsContext.InteractionDateCountStatistics)}", true);
                break;
            case LogType.MessageDeleted:
                await UpdateStatisticsRecord<FileExtensionStatistic>($"Statistics.{nameof(StatisticsContext.FileExtensionStatistics)}", true);
                break;
        }

        await StatisticsContext.SaveChangesAsync();

        await UpdateStatisticsRecord<DatabaseStatistic>($"Statistics.{nameof(StatisticsContext.DatabaseStatistics)}", true);
        await StatisticsContext.SaveChangesAsync();
    }

    private async Task UpdateStatisticsRecord<TEntity>(string tableName, bool useStatisticsContext = false) where TEntity : class
    {
        var stats = await GetOrCreateStatisticEntity<DatabaseStatistic>(o => o.TableName == tableName, tableName);
        DbContext context = useStatisticsContext ? StatisticsContext : Context;

        stats.RecordsCount = await context.Set<TEntity>().LongCountAsync();
    }
}

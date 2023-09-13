using AuditLogService.Core.Entity;
using AuditLogService.Core.Entity.Statistics;
using GrillBot.Core.Database;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Core.Providers;

public class StatisticsProvider : IStatisticsProvider
{
    private AuditLogServiceContext Context { get; }
    private AuditLogStatisticsContext StatisticsContext { get; }

    public StatisticsProvider(AuditLogServiceContext context, AuditLogStatisticsContext statisticsContext)
    {
        Context = context;
        StatisticsContext = statisticsContext;
    }

    public async Task<Dictionary<string, long>> GetTableStatisticsAsync()
    {
        return new Dictionary<string, long>
        {
            { nameof(Context.ApiRequests), await Context.ApiRequests.LongCountAsync() },
            { nameof(Context.ChannelCreatedItems), await Context.ChannelCreatedItems.LongCountAsync() },
            { nameof(Context.ChannelDeletedItems), await Context.ChannelDeletedItems.LongCountAsync() },
            { nameof(Context.ChannelInfoItems), await Context.ChannelInfoItems.LongCountAsync() },
            { nameof(Context.ChannelUpdatedItems), await Context.ChannelUpdatedItems.LongCountAsync() },
            { nameof(Context.DeletedEmotes), await Context.DeletedEmotes.LongCountAsync() },
            { nameof(Context.EmbedFields), await Context.EmbedFields.LongCountAsync() },
            { nameof(Context.EmbedInfoItems), await Context.EmbedInfoItems.LongCountAsync() },
            { nameof(Context.Files), await Context.Files.LongCountAsync() },
            { nameof(Context.GuildInfoItems), await Context.GuildInfoItems.LongCountAsync() },
            { nameof(Context.GuildUpdatedItems), await Context.GuildUpdatedItems.LongCountAsync() },
            { nameof(Context.InteractionCommands), await Context.InteractionCommands.LongCountAsync() },
            { nameof(Context.JobExecutions), await Context.JobExecutions.LongCountAsync() },
            { nameof(Context.LogItems), await Context.LogItems.LongCountAsync() },
            { nameof(Context.LogMessages), await Context.LogMessages.LongCountAsync() },
            { nameof(Context.MemberRoleUpdatedItems), await Context.MemberRoleUpdatedItems.LongCountAsync() },
            { nameof(Context.MessageDeletedItems), await Context.MessageDeletedItems.LongCountAsync() },
            { nameof(Context.MessageEditedItems), await Context.MessageEditedItems.LongCountAsync() },
            { nameof(Context.OverwriteCreatedItems), await Context.OverwriteCreatedItems.LongCountAsync() },
            { nameof(Context.OverwriteDeletedItems), await Context.OverwriteDeletedItems.LongCountAsync() },
            { nameof(Context.OverwriteInfoItems), await Context.OverwriteInfoItems.LongCountAsync() },
            { nameof(Context.OverwriteUpdatedItems), await Context.OverwriteUpdatedItems.LongCountAsync() },
            { nameof(Context.ThreadDeletedItems), await Context.ThreadDeletedItems.LongCountAsync() },
            { nameof(Context.ThreadInfoItems), await Context.ThreadInfoItems.LongCountAsync() },
            { nameof(Context.ThreadUpdatedItems), await Context.ThreadUpdatedItems.LongCountAsync() },
            { nameof(Context.Unbans), await Context.Unbans.LongCountAsync() },
            { nameof(Context.UserJoinedItems), await Context.UserJoinedItems.LongCountAsync() },
            { nameof(Context.UserLeftItems), await Context.UserLeftItems.LongCountAsync() },
            { nameof(Context.MemberUpdatedItems), await Context.MemberUpdatedItems.LongCountAsync() },
            { nameof(Context.MemberInfos), await Context.MemberInfos.LongCountAsync() },
            { nameof(Context.RoleDeleted), await Context.RoleDeleted.LongCountAsync() },
            { nameof(Context.RoleInfos), await Context.RoleInfos.LongCountAsync() },

            { $"Statistics.{nameof(StatisticsContext.DateCountStatistics)}", await StatisticsContext.DateCountStatistics.LongCountAsync() },
            { $"Statistics.{nameof(StatisticsContext.RequestStats)}", await StatisticsContext.RequestStats.LongCountAsync() },
            { $"Statistics.{nameof(StatisticsContext.DailyAvgTimes)}", await StatisticsContext.DailyAvgTimes.LongCountAsync() },
            { $"Statistics.{nameof(StatisticsContext.ApiUserActionStatistics)}", await StatisticsContext.ApiUserActionStatistics.LongCountAsync() },
            { $"Statistics.{nameof(StatisticsContext.InteractionUserActionStatistics)}", await StatisticsContext.InteractionUserActionStatistics.LongCountAsync() },
            { $"Statistics.{nameof(StatisticsContext.TypeStatistics)}", await StatisticsContext.TypeStatistics.LongCountAsync() },
            { $"Statistics.{nameof(StatisticsContext.DateStatistics)}", await StatisticsContext.DateStatistics.LongCountAsync() },
            { $"Statistics.{nameof(StatisticsContext.FileExtensionStatistics)}", await StatisticsContext.FileExtensionStatistics.LongCountAsync() },
            { $"Statistics.{nameof(StatisticsContext.InteractionStatistics)}", await StatisticsContext.InteractionStatistics.LongCountAsync() },
            { $"Statistics.{nameof(StatisticsContext.JobInfos)}", await StatisticsContext.JobInfos.LongCountAsync() }
        };
    }
}

using AuditLogService.Core.Entity;
using GrillBot.Core.Database;
using Microsoft.EntityFrameworkCore;

namespace AuditLogService.Core.Providers;

public class StatisticsProvider : IStatisticsProvider
{
    private AuditLogServiceContext Context { get; }

    public StatisticsProvider(AuditLogServiceContext context)
    {
        Context = context;
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
        };
    }
}

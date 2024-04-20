using Discord;
using Discord.Rest;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Cache;
using Microsoft.Extensions.Configuration;

#pragma warning disable S3928 // Parameter names used into ArgumentException constructors should match an existing one 
namespace GrillBot.Services.Common.Discord;

public class DiscordManager
{
    private DiscordRestClient DiscordClient { get; }
    private IConfiguration Configuration { get; }
    private ICounterManager CounterManager { get; }

    private AuditLogCache AuditLogCache { get; }
    private GuildCache GuildCache { get; }

    public DiscordManager(IDiscordClient discordClient, IConfiguration configuration, ICounterManager counterManager, GuildCache guildCache, AuditLogCache auditLogCache)
    {
        Configuration = configuration;
        CounterManager = counterManager;
        DiscordClient = (DiscordRestClient)discordClient;
        GuildCache = guildCache;
        AuditLogCache = auditLogCache;
    }

    public async Task LoginAsync()
    {
        var token = Configuration.GetConnectionString("BotToken")
            ?? throw new ArgumentNullException(nameof(Configuration));

        using (CounterManager.Create("Discord.API.Login"))
        {
            await DiscordClient.LoginAsync(TokenType.Bot, token);
        }
    }

    public async Task<IGuild?> GetGuildAsync(ulong guildId, bool forceApi = false)
    {
        if (!forceApi)
        {
            var cachedGuild = GuildCache.GetGuild(guildId);
            if (cachedGuild is not null)
                return cachedGuild;
        }

        using (CounterManager.Create("Discord.API.Guild"))
        {
            var guild = await DiscordClient.GetGuildAsync(guildId);
            if (guild is null)
                return null;

            GuildCache.StoreGuild(guild);
            return guild;
        }
    }

    public async Task<IEnumerable<IAuditLogEntry>> GetAuditLogsAsync(ulong guildId, int limit = DiscordConfig.MaxAuditLogEntriesPerBatch, ActionType? actionType = null)
    {
        if (actionType is not null)
        {
            var cachedLogs = AuditLogCache.GetAuditLogs(guildId, actionType.Value);
            if (cachedLogs is not null)
                return cachedLogs;
        }

        var guild = await GetGuildAsync(guildId);
        if (guild is null)
            return new List<IAuditLogEntry>();

        using (CounterManager.Create("Discord.API.AuditLogs"))
        {
            var logs = await guild.GetAuditLogsAsync(limit, actionType: actionType);

            if (actionType is not null && logs.Count > 0)
                AuditLogCache.StoreLogs(guildId, actionType.Value, logs);
            return logs;
        }
    }

    public async Task<IBan?> GetBanAsync(ulong guildId, ulong userId)
    {
        var guild = await GetGuildAsync(guildId);
        if (guild is null)
            return null;

        using (CounterManager.Create("Discord.API.Ban"))
        {
            return await guild.GetBanAsync(userId);
        }
    }
}

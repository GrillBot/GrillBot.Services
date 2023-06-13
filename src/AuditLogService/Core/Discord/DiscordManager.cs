using AuditLogService.Core.Discord.Cache;
using Discord;
using Discord.Rest;
using GrillBot.Core.Managers.Performance;

namespace AuditLogService.Core.Discord;

public sealed class DiscordManager : IDisposable
{
    private DiscordRestClient DiscordClient { get; }
    private IConfiguration Configuration { get; }
    private ICounterManager CounterManager { get; }

    private AuditLogCache AuditLogCache { get; }
    private GuildCache GuildCache { get; }

    public DiscordManager(IDiscordClient discordClient, IConfiguration configuration, ICounterManager counterManager)
    {
        Configuration = configuration;
        CounterManager = counterManager;
        DiscordClient = (DiscordRestClient)discordClient;

        AuditLogCache = new AuditLogCache(counterManager);
        GuildCache = new GuildCache(counterManager);
    }

    public async Task LoginAsync()
    {
        var token = Configuration.GetConnectionString("BotToken") ?? throw new ArgumentNullException(nameof(Configuration));

        using (CounterManager.Create("Discord.API.Login"))
        {
            await DiscordClient.LoginAsync(TokenType.Bot, token);
        }
    }

    public async Task<IGuild?> GetGuildAsync(ulong guildId)
    {
        var cachedGuild = GuildCache.GetGuild(guildId);
        if (cachedGuild is not null)
            return cachedGuild;

        using (CounterManager.Create("Discord.API.Guild"))
        {
            var guild = await DiscordClient.GetGuildAsync(guildId);
            if (guild is null)
                return null;

            GuildCache.StoreGuild(guildId, guild);
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

            if (actionType is not null)
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

    public void Dispose()
    {
        AuditLogCache.Dispose();
        GuildCache.Dispose();
    }
}

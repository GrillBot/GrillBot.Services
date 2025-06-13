using Discord;
using Discord.Net;
using Discord.Rest;
using GrillBot.Core.Managers.Performance;
using GrillBot.Services.Common.Cache;
using Microsoft.Extensions.Configuration;

#pragma warning disable S3928 // Parameter names used into ArgumentException constructors should match an existing one 
#pragma warning disable S3604 // Member initializer values should not be redundant
namespace GrillBot.Services.Common.Discord;

public class DiscordManager(
    IDiscordClient _discordClient,
    IConfiguration _configuration,
    ICounterManager _counterManager,
    GuildCache _guildCache,
    AuditLogCache _auditLogCache
)
{
    private DiscordRestClient DiscordClient { get; } = (DiscordRestClient)_discordClient;

    public IUser CurrentUser => _discordClient.CurrentUser;

    public async Task LoginAsync()
    {
        var token = _configuration.GetConnectionString("BotToken")
            ?? throw new ArgumentNullException(nameof(_configuration));

        using (_counterManager.Create("Discord.API.Login"))
        {
            await DiscordClient.LoginAsync(TokenType.Bot, token);
        }
    }

    public async Task<IGuild?> GetGuildAsync(ulong guildId, bool forceApi = false)
    {
        if (!forceApi)
        {
            var cachedGuild = _guildCache.GetGuild(guildId);
            if (cachedGuild is not null)
                return cachedGuild;
        }

        using (_counterManager.Create("Discord.API.Guild"))
        {
            var guild = await DiscordClient.GetGuildAsync(guildId);
            if (guild is null)
                return null;

            _guildCache.StoreGuild(guild);
            return guild;
        }
    }

    public async Task<IEnumerable<IAuditLogEntry>> GetAuditLogsAsync(ulong guildId, int limit = DiscordConfig.MaxAuditLogEntriesPerBatch, ActionType? actionType = null)
    {
        if (actionType is not null)
        {
            var cachedLogs = _auditLogCache.GetAuditLogs(guildId, actionType.Value);
            if (cachedLogs is not null)
                return cachedLogs;
        }

        var guild = await GetGuildAsync(guildId);
        if (guild is null)
            return [];

        using (_counterManager.Create("Discord.API.AuditLogs"))
        {
            var logs = await guild.GetAuditLogsAsync(limit, actionType: actionType);

            if (actionType is not null && logs.Count > 0)
                _auditLogCache.StoreLogs(guildId, actionType.Value, logs);
            return logs;
        }
    }

    public async Task<IBan?> GetBanAsync(ulong guildId, ulong userId)
    {
        var guild = await GetGuildAsync(guildId);
        if (guild is null)
            return null;

        using (_counterManager.Create("Discord.API.Ban"))
        {
            return await guild.GetBanAsync(userId);
        }
    }

    public async Task<List<IInviteMetadata>> GetInvitesAsync(ulong guildId)
    {
        var guild = await GetGuildAsync(guildId);
        if (guild is null)
            return [];

        var result = new List<IInviteMetadata>();

        using (_counterManager.Create("Discord.API.Invites"))
        {
            if (!string.IsNullOrEmpty(guild.VanityURLCode))
                result.Add(await guild.GetVanityInviteAsync());

            result.AddRange(await guild.GetInvitesAsync());
        }

        return result;
    }

    public async Task<IUser?> GetUserAsync(ulong userId)
    {
        using (_counterManager.Create("Discord.API.User"))
        {
            try
            {
                return await DiscordClient.GetUserAsync(userId);
            }
            catch (HttpException ex) when (ex.DiscordCode is DiscordErrorCode.UserBanned or DiscordErrorCode.UnknownUser)
            {
                return null;
            }
        }
    }
}

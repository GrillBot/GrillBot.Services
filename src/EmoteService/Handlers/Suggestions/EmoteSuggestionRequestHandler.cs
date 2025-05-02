using EmoteService.Core.Entity;
using EmoteService.Core.Entity.Suggestions;
using EmoteService.Models.Events.Suggestions;
using GrillBot.Core.Extensions;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Core.Services.AuditLog.Enums;
using GrillBot.Core.Services.AuditLog.Models.Events.Create;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;

namespace EmoteService.Handlers.Suggestions;

public partial class EmoteSuggestionRequestHandler(
    ILoggerFactory loggerFactory,
    EmoteServiceContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher rabbitPublisher
) : BaseEventHandlerWithDb<EmoteSuggestionRequestPayload, EmoteServiceContext>(loggerFactory, dbContext, counterManager, rabbitPublisher)
{
    [GeneratedRegex(@"\w+", RegexOptions.IgnoreCase)]
    private static partial Regex EmoteNameRegex();

    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(EmoteSuggestionRequestPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        var guild = await ValidateInputAndGetGuildAsync(message);
        if (guild is null)
            return RabbitConsumptionResult.Reject;

        var entity = await CreateSuggestionEntityAsync(message);

        var messages = new List<DiscordMessagePayload>
        {
            await CreateAdminChannelNotificationAsync(entity, guild),
            await CreateUserNotificationAsync(entity)
        };

        await Publisher.PublishAsync(messages);
        return RabbitConsumptionResult.Success;
    }

    private async Task<Guild?> ValidateInputAndGetGuildAsync(EmoteSuggestionRequestPayload message)
    {
        try
        {
            ArgumentException.ThrowIfNullOrEmpty(message.Name);

            if (!EmoteNameRegex().IsMatch(message.Name))
                throw new ArgumentException($"Emote name ({message.Name}) does not meet naming rules.", nameof(message));

            ArgumentException.ThrowIfNullOrEmpty(message.ReasonToAdd);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(message.ReasonToAdd.Length, 4000);

            ArgumentNullException.ThrowIfNull(message.Image);
            ArgumentOutOfRangeException.ThrowIfLessThan(message.Image.Length, 5);

            ArgumentException.ThrowIfNullOrEmpty(message.GuildId);
            if (!ulong.TryParse(message.GuildId, CultureInfo.InvariantCulture, out var guildId))
                throw new ArgumentException($"Provided GuildId ({message.GuildId}) is not valid guild ID.", nameof(message));

            ArgumentException.ThrowIfNullOrEmpty(message.FromUserId);
            if (!ulong.TryParse(message.FromUserId, CultureInfo.InvariantCulture, out _))
                throw new ArgumentException($"Provided FromUserId ({message.FromUserId}) is not valid user ID.", nameof(message));

            var guild = await DbContext.Guilds.AsNoTracking()
                .FirstOrDefaultAsync(o => o.GuildId == guildId);

            ValidateConfiguration(guild);
            return guild;
        }
        catch (ArgumentException ex)
        {
            var logRequest = new LogRequest(LogType.Warning, message.CreatedAtUtc, message.GuildId, message.FromUserId)
            {
                LogMessage = new LogMessageRequest
                {
                    Message = ex.Message,
                    Source = nameof(EmoteSuggestionRequestHandler),
                    SourceAppName = "EmoteService"
                }
            };

            await Publisher.PublishAsync(new CreateItemsMessage(logRequest));
            return null;
        }
    }

    private static void ValidateConfiguration(Guild? guild)
    {
        if (guild is null)
            throw new ArgumentException("Missing guild configuration", nameof(guild));

        if (guild.SuggestionChannelId == default(ulong))
            throw new ArgumentException("Missing suggestion channel configuration.", nameof(guild));
    }

    private async Task<EmoteSuggestion> CreateSuggestionEntityAsync(EmoteSuggestionRequestPayload message)
    {
        var entity = new EmoteSuggestion
        {
            ApprovedForVote = false,
            FromUserId = message.FromUserId.ToUlong(),
            GuildId = message.GuildId.ToUlong(),
            Image = message.Image,
            IsAnimated = message.IsAnimated,
            Name = message.Name,
            ReasonForAdd = message.ReasonToAdd,
            SuggestedAtUtc = message.CreatedAtUtc
        };

        await DbContext.AddAsync(entity);
        await DbContext.SaveChangesAsync();

        return entity;
    }

    private async Task<DiscordMessagePayload> CreateAdminChannelNotificationAsync(EmoteSuggestion suggestion, Guild guild)
    {
        return new DiscordMessagePayload(); // TODO
    }

    private async Task<DiscordMessagePayload> CreateUserNotificationAsync(EmoteSuggestion suggestion)
    {
        return new DiscordMessagePayload(); // TODO
    }
}

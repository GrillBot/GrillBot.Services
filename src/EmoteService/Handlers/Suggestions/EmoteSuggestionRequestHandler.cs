using Discord;
using EmoteService.Core.Entity.Suggestions;
using EmoteService.Core.Options;
using EmoteService.Models.Events.Suggestions;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.Services.AuditLog.Enums;
using GrillBot.Core.Services.AuditLog.Models.Events.Create;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace EmoteService.Handlers.Suggestions;

public partial class EmoteSuggestionRequestHandler(
    IServiceProvider serviceProvider,
    IOptions<AppOptions> _options
) : EmoteSuggestionHandlerBase<EmoteSuggestionRequestPayload>(serviceProvider)
{
    [GeneratedRegex(@"\w+", RegexOptions.IgnoreCase)]
    private static partial Regex EmoteNameRegex();

    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(EmoteSuggestionRequestPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        var guild = await ValidateInputAndGetGuildAsync(message);
        if (guild is null)
            return RabbitConsumptionResult.Reject;

        var entity = await CreateSuggestionEntityAsync(message);

        var messages = new List<DiscordSendMessagePayload>
        {
            (DiscordSendMessagePayload)CreateAdminChannelNotification(entity, guild, null),
            CreateUserNotification(entity, message.Locale)
        };

        await Publisher.PublishAsync(messages);
        return RabbitConsumptionResult.Success;
    }

    private async Task<Core.Entity.Guild?> ValidateInputAndGetGuildAsync(EmoteSuggestionRequestPayload message)
    {
        try
        {
            ArgumentException.ThrowIfNullOrEmpty(message.Name);

            if (!EmoteNameRegex().IsMatch(message.Name))
                throw new ArgumentException($"Emote name ({message.Name}) does not meet naming rules.", nameof(message));

            ArgumentException.ThrowIfNullOrEmpty(message.ReasonToAdd);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(message.ReasonToAdd.Length, EmbedFieldBuilder.MaxFieldValueLength);
            ArgumentNullException.ThrowIfNull(message.Image);
            ArgumentOutOfRangeException.ThrowIfLessThan(message.Image.Length, 5);
            ArgumentOutOfRangeException.ThrowIfZero(message.GuildId);
            ArgumentOutOfRangeException.ThrowIfZero(message.FromUserId);

            var guildQuery = DbContext.Guilds.AsNoTracking()
                .Where(o => o.GuildId == message.GuildId);

            var guild = await ContextHelper.ReadFirstOrDefaultEntityAsync(guildQuery);
            ValidateConfiguration(guild);

            if (!await CheckUserQuotaAsync(message))
                return null;

            return guild;
        }
        catch (ArgumentException ex)
        {
            var logRequest = new LogRequest(LogType.Warning, message.CreatedAtUtc, message.GuildId.ToString(), message.FromUserId.ToString())
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

    private static void ValidateConfiguration(Core.Entity.Guild? guild)
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
            FromUserId = message.FromUserId,
            GuildId = message.GuildId,
            Image = message.Image,
            IsAnimated = message.IsAnimated,
            Name = message.Name,
            ReasonForAdd = message.ReasonToAdd,
            SuggestedAtUtc = message.CreatedAtUtc
        };

        await DbContext.AddAsync(entity);
        await ContextHelper.SaveChangesAsync();

        return entity;
    }

    private static DiscordSendMessagePayload CreateUserNotification(EmoteSuggestion suggestion, string locale)
    {
        var message = new DiscordSendMessagePayload(
            null,
            suggestion.FromUserId,
            "SuggestionModule/PrivateMessageSuggestionCreated",
            [],
            "Emote"
        );

        message.WithLocalization(locale: locale);
        return message;
    }

    private async Task<bool> CheckUserQuotaAsync(EmoteSuggestionRequestPayload request)
    {
        var query = DbContext.EmoteSuggestions.AsNoTracking()
            .Where(o => o.FromUserId == request.FromUserId && o.GuildId == request.GuildId && o.SuggestedAtUtc.Date == DateTime.UtcNow.Date);

        var count = await ContextHelper.ReadCountAsync(query);
        if (count < _options.Value.Suggestions.MaxSuggestionsPerUserPerDay)
            return true;

        var msg = new DiscordSendMessagePayload(request.GuildId, request.FromUserId, "SuggestionModule/CreateSuggestion/TooMuchSuggestions", [], "Emote");
        msg.WithLocalization(locale: request.Locale);

        await Publisher.PublishAsync(msg);

        return false;
    }
}

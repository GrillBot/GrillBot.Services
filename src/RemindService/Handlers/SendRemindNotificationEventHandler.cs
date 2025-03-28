﻿using Discord;
using GrillBot.Core.Infrastructure.Auth;
using GrillBot.Core.Managers.Performance;
using GrillBot.Core.RabbitMQ.V2.Consumer;
using GrillBot.Core.RabbitMQ.V2.Publisher;
using GrillBot.Core.Services.GrillBot.Models.Events.Messages;
using GrillBot.Services.Common.Infrastructure.RabbitMQ;
using RemindService.Core.Entity;
using RemindService.Models.Events;
using RemindService.Options;
using Components = GrillBot.Core.Services.GrillBot.Models.Events.Messages.Components;

namespace RemindService.Handlers;

public class SendRemindNotificationEventHandler(
    ILoggerFactory loggerFactory,
    RemindServiceContext dbContext,
    ICounterManager counterManager,
    IRabbitPublisher publisher
) : BaseEventHandlerWithDb<SendRemindNotificationPayload, RemindServiceContext>(loggerFactory, dbContext, counterManager, publisher)
{
    protected override async Task<RabbitConsumptionResult> HandleInternalAsync(SendRemindNotificationPayload message, ICurrentUserProvider currentUser, Dictionary<string, string> headers)
    {
        var remindMessage = await GetRemindMessageAsync(message.RemindId);
        if (remindMessage is null)
            return RabbitConsumptionResult.Success;

        var discordMessage = ProcessRemind(remindMessage, message.IsEarly);

        remindMessage.IsSendInProgress = true;
        await ContextHelper.SaveChagesAsync();
        await Publisher.PublishAsync(discordMessage);
        return RabbitConsumptionResult.Success;
    }

    private async Task<RemindMessage?> GetRemindMessageAsync(int id)
    {
        var query = DbContext.RemindMessages.Where(o => o.Id == id && !o.IsSendInProgress && string.IsNullOrEmpty(o.NotificationMessageId));
        return await ContextHelper.ReadFirstOrDefaultEntityAsync(query);
    }

    private static DiscordMessagePayload ProcessRemind(RemindMessage remindMessage, bool isEarly)
    {
        var embed = CreateRemindEmbed(remindMessage, isEarly);
        var postponeComponents = CreatePostponeComponents(isEarly);
        var attachments = Enumerable.Empty<DiscordMessageFile>();
        var message = new DiscordMessagePayload(null, remindMessage.ToUserId, null, attachments, "Remind", null, null, embed, null, postponeComponents);

        message.ServiceData.Add("UseLocalizedEmbeds", "true");
        message.ServiceData.Add("RemindId", remindMessage.Id.ToString());
        message.ServiceData.Add("Language", remindMessage.Language);
        message.ServiceData.Add("PostponeCount", remindMessage.PostponeCount.ToString());

        return message;
    }

    private static DiscordMessageEmbed CreateRemindEmbed(RemindMessage message, bool isEarly)
    {
        const string prefix = "RemindModule/NotifyMessage/";

        return new DiscordMessageEmbed(
            url: null,
            title: prefix + (isEarly ? "ForceTitle" : "Title"),
            description: null,
            author: new DiscordMessageEmbedAuthor
            {
                IconUrl = "Bot.AvatarUrl",
                Name = "Bot.DisplayName"
            },
            color: (isEarly ? Color.Gold : Color.Green).RawValue,
            footer: null,
            imageUrl: null,
            thumbnailUrl: null,
            fields: CreateEmbedFields(message, isEarly, prefix),
            timestamp: null,
            useCurrentTimestamp: true
        );
    }

    private static IEnumerable<DiscordMessageEmbedField> CreateEmbedFields(RemindMessage message, bool isEarly, string prefix)
    {
        yield return new(prefix + "Fields/Id", message.Id.ToString(), true);

        if (message.FromUserId != message.ToUserId)
            yield return new(prefix + "Fields/From", $"UserDisplayName.{message.FromUserId}", true);

        if (message.PostponeCount > 0)
            yield return new(prefix + "Fields/Attention", prefix + "Postponed", false);

        yield return new(prefix + "Fields/Message", message.Message, false);

        if (!isEarly)
            yield return new(prefix + "Fields/Options", prefix + "Options", false);
    }

    private static DiscordMessageComponent? CreatePostponeComponents(bool isEarly)
    {
        var removalButton = new Components.ButtonComponent(null, "remove_remind", ButtonStyle.Danger, ":wastebasket:");
        var components = new DiscordMessageComponent();

        if (isEarly)
        {
            components.AddButton(removalButton);
            return components;
        }

        foreach (var hour in AppOptions.PostponeHours)
            components.AddButton(new Components.ButtonComponent(null, $"remind_postpone:{hour.Key}", ButtonStyle.Primary, $":{hour.Value}:"));
        components.AddButton(removalButton);

        return components;
    }
}

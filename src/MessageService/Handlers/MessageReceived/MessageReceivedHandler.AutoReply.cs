using GrillBot.Core.Services.GrillBot.Models.Events.Messages;
using MessageService.Models.Events;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace MessageService.Handlers.MessageReceived;

public partial class MessageReceivedHandler
{
    private async Task ProcessAutoReplyAsync(MessageReceivedPayload message)
    {
        if (message.IsCommand() || !message.Author.IsUser() || string.IsNullOrEmpty(message.Content))
            return;

        var query = DbContext.AutoReplyDefinitions.AsNoTracking()
            .Where(o => !o.IsDeleted && !o.IsDisabled);
        var definitions = await ContextHelper.ReadEntitiesAsync(query);
        if (definitions.Count == 0)
            return;

        var match = definitions.Find(o => Regex.IsMatch(message.Content, o.Template, o.RegexOptions));
        if (match is null)
            return;

        var replyPayload = new DiscordSendMessagePayload(
            message.GuildId,
            message.ChannelId,
            match.Reply,
            [],
            "Message",
            reference: new(message.Id, message.ChannelId, message.GuildId, false, Discord.MessageReferenceType.Default)
        );

        await Publisher.PublishAsync(replyPayload);
    }
}

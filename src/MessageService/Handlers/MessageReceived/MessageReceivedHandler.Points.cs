using GrillBot.Core.Services.PointsService.Models;
using GrillBot.Core.Services.PointsService.Models.Events;
using MessageService.Models.Events;

namespace MessageService.Handlers.MessageReceived;

public partial class MessageReceivedHandler
{
    private Task ProcessPointsTransactionRequestAsync(MessageReceivedPayload message)
    {
        if (!message.Author.IsUser() || message.IsCommand())
            return Task.CompletedTask;

        var messageInfo = new MessageInfo
        {
            AuthorId = message.Author.Id.ToString(),
            ContentLength = message.Content?.Length ?? 0,
            Id = message.Id.ToString(),
            MessageType = message.Type
        };

        var payload = new CreateTransactionPayload(
            message.GuildId.ToString(),
            message.CreatedAt.UtcDateTime,
            message.ChannelId.ToString(),
            messageInfo
        );

        return Publisher.PublishAsync(payload);
    }
}

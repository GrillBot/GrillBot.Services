using MessageService.Models.Events;
using PointsService.Models;
using PointsService.Models.Events;

namespace MessageService.Handlers.MessageReceived;

public partial class MessageReceivedHandler
{
    private Task ProcessPointsTransactionRequestAsync(MessageReceivedPayload message)
    {
        if (!message.Author.IsUser() || message.IsCommand())
            return Task.CompletedTask;

        var payload = new CreateTransactionPayload(
            message.GuildId.ToString(),
            message.CreatedAt.UtcDateTime,
            message.ChannelId.ToString(),
            new MessageInfo
            {
                AuthorId = message.Author.Id.ToString(),
                ContentLength = message.Content?.Length ?? 0,
                Id = message.Id.ToString(),
                MessageType = message.Type
            }
        );

        return Publisher.PublishAsync(payload);
    }
}

using Discord;
using GrillBot.Core.RabbitMQ.V2.Messages;
using MessageService.Models.Events.Users;

namespace MessageService.Models.Events;

public class MessageReceivedPayload : IRabbitMessage
{
    public string Topic => "Message";
    public string Queue => "MessageReceived";

    public ulong Id { get; set; }
    public ulong ChannelId { get; set; }
    public ulong GuildId { get; set; }
    public UserPayload Author { get; set; } = null!;
    public MessageType Type { get; set; }
    public MessageSource Source { get; set; }
    public string? Content { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public MessageReceivedPayload()
    {
    }

    public MessageReceivedPayload(
        ulong id,
        ulong channelId,
        ulong guildId,
        UserPayload author,
        MessageType type,
        MessageSource source,
        string? content,
        DateTimeOffset createdAt
    )
    {
        Id = id;
        ChannelId = channelId;
        GuildId = guildId;
        Author = author;
        Type = type;
        Source = source;
        Content = content;
        CreatedAt = createdAt;
    }

    public bool IsCommand()
        => Type is MessageType.ApplicationCommand or MessageType.ContextMenuCommand;

    public static MessageReceivedPayload? Create(IMessage message)
    {
        if (message.Channel is not IGuildChannel guildChannel)
            return null;

        return new MessageReceivedPayload(
            message.Id,
            message.Channel.Id,
            guildChannel.GuildId,
            UserPayload.Create(message.Author),
            message.Type,
            message.Source,
            message.Content,
            message.CreatedAt
        );
    }
}

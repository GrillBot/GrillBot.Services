using Discord;

namespace MessageService.Models.Events.Users;

public class UserPayload
{
    public ulong Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsBot { get; set; }
    public bool IsWebhook { get; set; }

    public UserPayload()
    {
    }

    public UserPayload(ulong id, DateTimeOffset createdAt, bool isBot, bool isWebhook)
    {
        Id = id;
        CreatedAt = createdAt;
        IsBot = isBot;
        IsWebhook = isWebhook;
    }

    public bool IsUser()
        => !IsBot && !IsWebhook;

    public static UserPayload Create(IUser user)
        => new(user.Id, user.CreatedAt, user.IsBot, user.IsWebhook);
}

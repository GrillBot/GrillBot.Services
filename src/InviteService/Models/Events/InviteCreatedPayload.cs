using Discord;
using GrillBot.Core.RabbitMQ.V2.Messages;

namespace InviteService.Models.Events;

public class InviteCreatedPayload : IRabbitMessage
{
    public string Topic => "Invite";
    public string Queue => "InviteCreated";

    public string Code { get; set; } = null!;
    public string GuildId { get; set; } = null!;
    public int Uses { get; set; }
    public string? CreatorId { get; set; }
    public DateTime? CreatedAt { get; set; }

    public InviteCreatedPayload()
    {
    }

    public InviteCreatedPayload(string code, string guildId, int uses, string? creatorId, DateTime? createdAt)
    {
        Code = code;
        GuildId = guildId;
        Uses = uses;
        CreatorId = creatorId;
        CreatedAt = createdAt;
    }

    public InviteCreatedPayload(IInviteMetadata metadata) : this(
        metadata.Code,
        metadata.GuildId.ToString()!,
        metadata.Uses ?? 0,
        metadata.Inviter?.Id.ToString(),
        metadata.CreatedAt?.UtcDateTime
    )
    { }
}

using GrillBot.Core.RabbitMQ.V2.Messages;

namespace InviteService.Models.Events;

public class SynchronizeGuildInvitesPayload : IRabbitMessage
{
    public string Topic => "Invite";
    public string Queue => "SynchronizeGuildInvites";

    public string GuildId { get; set; } = null!;

    public SynchronizeGuildInvitesPayload()
    {
    }

    public SynchronizeGuildInvitesPayload(string guildId)
    {
        GuildId = guildId;
    }
}

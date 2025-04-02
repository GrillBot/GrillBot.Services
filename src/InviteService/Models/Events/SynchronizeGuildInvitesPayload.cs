using GrillBot.Core.RabbitMQ.V2.Messages;

namespace InviteService.Models.Events;

public class SynchronizeGuildInvitesPayload : IRabbitMessage
{
    public string Topic => "Invite";
    public string Queue => "SynchronizeGuildInvites";

    public string GuildId { get; set; } = null!;
    public bool IgnoreLog { get; set; }

    public SynchronizeGuildInvitesPayload()
    {
    }

    public SynchronizeGuildInvitesPayload(string guildId, bool ignoreLog)
    {
        GuildId = guildId;
        IgnoreLog = ignoreLog;
    }
}

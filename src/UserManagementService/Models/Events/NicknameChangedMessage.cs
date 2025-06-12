using Discord;
using GrillBot.Core.RabbitMQ.V2.Messages;

namespace UserManagementService.Models.Events;

public class NicknameChangedMessage : IRabbitMessage
{
    public string Topic => "UserManagement";
    public string Queue => "NicknameChanged";

    public ulong GuildId { get; set; }
    public ulong UserId { get; set; }
    public string? NicknameBefore { get; set; }
    public string? NicknameAfter { get; set; }

    public NicknameChangedMessage(ulong guildId, ulong userId, string? nicknameBefore, string? nicknameAfter)
    {
        GuildId = guildId;
        UserId = userId;
        NicknameBefore = nicknameBefore;
        NicknameAfter = nicknameAfter;
    }

    public NicknameChangedMessage()
    {
    }

    public static NicknameChangedMessage Create(IGuildUser userBefore, IGuildUser userAfter)
        => new(userBefore.Guild.Id, userBefore.Id, userBefore.Nickname, userAfter.Nickname);
}

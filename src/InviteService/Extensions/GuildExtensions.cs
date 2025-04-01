using Discord;

namespace InviteService.Extensions;

public static class GuildExtensions
{
    public static async Task<bool> CanManageInvitesAsync(this IGuild guild, IUser user)
    {
        var guildUser = user as IGuildUser ?? await guild.GetUserAsync(user.Id);
        return guildUser?.CanManageInvites() == true;
    }
}

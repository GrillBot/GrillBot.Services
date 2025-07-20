using Discord;

namespace InviteService.Extensions;

public static class GuildExtensions
{
    public static async Task<bool> CanManageInvitesAsync(this IGuild guild, IUser user, CancellationToken cancellationToken = default)
    {
        var guildUser = user as IGuildUser ?? await guild.GetUserAsync(user.Id, options: new() { CancelToken = cancellationToken });
        return guildUser?.CanManageInvites() == true;
    }
}

using Discord;

namespace InviteService.Extensions;

public static class GuildUserExtensions
{
    public static bool CanManageInvites(this IGuildUser user)
        => user.GuildPermissions.CreateInstantInvite && user.GuildPermissions.ManageGuild;
}

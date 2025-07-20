using Discord;

namespace GrillBot.Services.Common.Extensions;

public static class GuildUserExtensions
{
    public static IEnumerable<IRole> GetRoles(this IGuildUser user, bool withEveryone = false)
    {
        var ids = withEveryone ? user.RoleIds : user.RoleIds.Where(o => user.Guild.EveryoneRole.Id != o);
        return ids.Select(user.Guild.GetRole).Where(o => o is not null);
    }
}

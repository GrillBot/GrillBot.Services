using Discord;

namespace SearchingService.Models.Events.Users;

public class UserSynchronizationItem
{
    public string GuildId { get; set; } = null!;
    public string UserId { get; set; } = null!;

    public bool IsAdmin { get; set; }
    public GuildPermission GuildPermissions { get; set; }

    public UserSynchronizationItem()
    {
    }

    public UserSynchronizationItem(string guildId, string userId, bool isAdmin, GuildPermission permissions)
    {
        GuildId = guildId;
        UserId = userId;
        IsAdmin = isAdmin;
        GuildPermissions = permissions;
    }
}

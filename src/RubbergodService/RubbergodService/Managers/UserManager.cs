using Discord;
using RubbergodService.Core.Entity;
using RubbergodService.Core.Repository;
using RubbergodService.Discord;

namespace RubbergodService.Managers;

public class UserManager
{
    private RubbergodServiceRepository Repository { get; }
    private DiscordManager DiscordManager { get; }

    public UserManager(RubbergodServiceRepository repository, DiscordManager discordManager)
    {
        Repository = repository;
        DiscordManager = discordManager;
    }

    /// <summary>
    /// Check if member exists in the database and download if need it.
    /// </summary>
    /// <param name="memberId">Member ID</param>
    public async Task InitMemberAsync(string memberId)
    {
        var member = await Repository.MemberCache.FindMemberByIdAsync(memberId);
        if (member is null)
        {
            member = new MemberCacheItem { UserId = memberId };
            await Repository.AddAsync(member);
        }

        var user = await DiscordManager.GetUserAsync(Convert.ToUInt64(memberId));
        if (user is null)
        {
            member.Username = "Deleted user";
            member.Discriminator = "0000";
            member.AvatarUrl = CDN.GetDefaultUserAvatarUrl(0);
        }
        else
        {
            member.Username = user.Username;
            member.Discriminator = user.Discriminator;
            member.AvatarUrl = user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl();
        }

        await Repository.CommitAsync();
    }
}

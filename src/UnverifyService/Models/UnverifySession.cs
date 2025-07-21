using Discord;

namespace UnverifyService.Models;

public record UnverifySession(
    IGuildUser TargetUser,
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    string? Reason,
    bool IsSelfUnverify
)
{
    public List<IRole> RolesToRemove { get; } = [];
    public List<IRole> RolesToKeep { get; } = [];
    public List<(IGuildChannel, OverwritePermissions)> ChannelsToRemove { get; } = [];
    public List<(IGuildChannel, OverwritePermissions)> ChannelsToKeep { get; } = [];
    public bool KeepMutedRole { get; set; }
}

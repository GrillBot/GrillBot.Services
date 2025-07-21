using Discord;
using UnverifyService.Core.Entity;

namespace UnverifyService.Models;

public record UnverifySession(
    IGuildUser TargetUser,
    User? TargetUserEntity,
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
    public IRole? MutedRole { get; set; }

    public bool KeepMutedRole => MutedRole is not null && RolesToKeep.Exists(o => o.Id == MutedRole.Id);
}

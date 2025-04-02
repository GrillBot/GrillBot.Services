using Discord;

namespace InviteService.Models.Cache;

public record InviteMetadata(
    string Code,
    int Uses,
    string? CreatorId,
    DateTime? CreatedAt
)
{
    public static InviteMetadata Create(IInviteMetadata metadata)
    {
        return new(metadata.Code, metadata.Uses ?? 0, metadata.Inviter?.Id.ToString(), metadata.CreatedAt?.UtcDateTime);
    }
}

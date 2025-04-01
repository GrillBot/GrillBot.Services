namespace InviteService.Models.Cache;

public record InviteMetadata(
    string Code,
    int Uses,
    string? CreatorId,
    DateTime? CreatedAt
);

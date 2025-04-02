namespace InviteService.Models.Response;

public record Invite(
    string Code,
    string GuildId,
    string? CreatorId,
    DateTime? CreatedAt,
    int Uses
);

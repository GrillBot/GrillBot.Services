namespace InviteService.Models.Response;

public record UserInviteUse(
    string GuildId,
    string Code,
    DateTime JoinedAtUtc
);

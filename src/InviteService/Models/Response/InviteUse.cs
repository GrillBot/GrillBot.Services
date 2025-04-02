namespace InviteService.Models.Response;

public record InviteUse(
    string UserId,
    DateTime UsedAtUtc
);

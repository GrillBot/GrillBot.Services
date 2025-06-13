namespace UserManagementService.Models.Response;

public record UserInfo(
    string UserId,
    List<GuildUser> Guilds
);

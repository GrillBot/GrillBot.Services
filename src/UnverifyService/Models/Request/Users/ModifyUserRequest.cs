namespace UnverifyService.Models.Request.Users;

public class ModifyUserRequest
{
    public TimeSpan? SelfUnverifyMinimalTime { get; set; }
    public bool IsBotAdmin { get; set; }
}

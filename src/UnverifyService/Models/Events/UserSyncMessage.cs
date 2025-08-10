namespace UnverifyService.Models.Events;

public class UserSyncMessage
{
    public ulong UserId { get; set; }
    public bool IsBot { get; set; }
    public string? UserLanguage { get; set; }
}

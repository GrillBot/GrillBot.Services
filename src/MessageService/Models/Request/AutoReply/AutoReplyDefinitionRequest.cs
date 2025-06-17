namespace MessageService.Models.Request.AutoReply;

public class AutoReplyDefinitionRequest
{
    public string Template { get; set; } = null!;
    public string Reply { get; set; } = null!;
    public bool IsDisabled { get; set; }
    public bool IsCaseSensitive { get; set; } = false;
}

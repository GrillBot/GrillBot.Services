namespace UnverifyService.Models.Request;

public class CreateKeepableRequest
{
    public string Group { get; set; } = "-";
    public string Name { get; set; } = null!;
}

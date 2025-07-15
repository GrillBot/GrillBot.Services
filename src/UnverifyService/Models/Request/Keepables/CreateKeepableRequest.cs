using System.ComponentModel.DataAnnotations;

namespace UnverifyService.Models.Request.Keepables;

public class CreateKeepableRequest
{
    [MaxLength(100)]
    public string Group { get; set; } = "-";

    [MaxLength(100)]
    public string Name { get; set; } = null!;
}

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RubbergodService.Models;

public class DirectApiCommand
{
    [Required]
    [JsonPropertyName("method")]
    public string Method { get; set; } = null!;

    [JsonPropertyName("parameters")]
    public Dictionary<string, object> Parameters { get; set; }

    public DirectApiCommand()
    {
        Parameters = new Dictionary<string, object>();
    }
}

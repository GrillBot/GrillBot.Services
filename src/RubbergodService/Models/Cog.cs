using System.Text.Json.Serialization;

namespace RubbergodService.Models;

public class Cog
{
    [JsonPropertyName("id")]
    public ulong? Id { get; set; }

    [JsonPropertyName("children")]
    public List<string> Children { get; set; } = [];
}

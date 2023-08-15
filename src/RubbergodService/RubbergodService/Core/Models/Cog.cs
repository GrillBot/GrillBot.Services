using System.Text.Json.Serialization;

namespace RubbergodService.Core.Models;

public class Cog
{
    [JsonPropertyName("id")]
    public ulong? Id { get; set; }

    [JsonPropertyName("children")]
    public List<string> Children { get; set; } = new();
}

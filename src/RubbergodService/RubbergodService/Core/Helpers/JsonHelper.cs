using System.Text;
using System.Text.Json;

namespace RubbergodService.Core.Helpers;

public static class JsonHelper
{
    public static async Task<string> SerializeJsonDocumentAsync(JsonDocument document)
    {
        using var stream = new MemoryStream();
        var writer = new Utf8JsonWriter(stream);

        document.WriteTo(writer);
        await writer.FlushAsync();

        return Encoding.UTF8.GetString(stream.ToArray());
    }
}

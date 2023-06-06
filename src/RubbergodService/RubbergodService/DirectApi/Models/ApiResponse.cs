namespace RubbergodService.DirectApi.Models;

public class ApiResponse
{
    public string Filename { get; set; } = null!;
    public byte[] Content { get; set; } = null!;
}

namespace UnverifyService.Models.Response.Logs;

public record UpdatePreview(
    DateTime NewStartAtUtc,
    DateTime NewEndAtUtc,
    string? Reason = null
);

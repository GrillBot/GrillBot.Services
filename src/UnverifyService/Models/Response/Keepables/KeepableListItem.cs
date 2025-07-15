namespace UnverifyService.Models.Response.Keepables;

public record KeepableListItem(
    string Group,
    string Name,
    DateTime CreatedAtUtc
);

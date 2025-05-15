#pragma warning disable S2094 // Classes should not be empty
namespace EmoteService.Core.Options;

public class AppOptions
{
    public required SuggestionsOptions Suggestions { get; set; }
}

using Discord;
using GrillBot.Core.RabbitMQ.V2.Messages;

namespace EmoteService.Models.Events.Suggestions;

public class EmoteSuggestionUserVotePayload : IRabbitMessage
{
    public string Topic => "Emote";
    public string Queue => "EmoteSuggestionUserVote";

    public Guid SuggestionId { get; set; }
    public bool IsApproved { get; set; }
    public ulong UserId { get; set; }

    public EmoteSuggestionUserVotePayload()
    {
    }

    public EmoteSuggestionUserVotePayload(Guid suggestionId, bool isApproved, ulong userId)
    {
        SuggestionId = suggestionId;
        IsApproved = isApproved;
        UserId = userId;
    }

    public static EmoteSuggestionUserVotePayload Create(Guid suggestionId, bool isApproved, IUser user)
        => new(suggestionId, isApproved, user.Id);
}

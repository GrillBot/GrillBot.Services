using Discord;
using GrillBot.Core.RabbitMQ.V2.Messages;

namespace EmoteService.Models.Events.Suggestions;

public class EmoteSuggestionApprovalChangePayload : IRabbitMessage
{
    public string Topic => "Emote";
    public string Queue => "EmoteSuggestionApprovalChange";

    public Guid SuggestionId { get; set; }
    public bool IsApprovedForVote { get; set; }
    public ulong ApprovedByUserId { get; set; }

    public EmoteSuggestionApprovalChangePayload()
    {
    }

    public EmoteSuggestionApprovalChangePayload(Guid suggestionId, bool isApprovedForVote, ulong approvedByUserId)
    {
        SuggestionId = suggestionId;
        IsApprovedForVote = isApprovedForVote;
        ApprovedByUserId = approvedByUserId;
    }

    public static EmoteSuggestionApprovalChangePayload Create(Guid suggestionId, bool isApprovedForVote, IUser approvedBy)
        => new(suggestionId, isApprovedForVote, approvedBy.Id);
}

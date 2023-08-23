using System.ComponentModel.DataAnnotations;
using GrillBot.Core.Validation;
using PointsService.Core.Repository;
using PointsService.Models;

namespace PointsService.Validation;

public class TransactionRequestValidator : ModelValidator<TransactionRequest>
{
    private PointsServiceRepository Repository { get; }

    public TransactionRequestValidator(PointsServiceRepository repository)
    {
        Repository = repository;
    }

    protected override IEnumerable<Func<TransactionRequest, ValidationContext, IEnumerable<ValidationResult>>> GetValidations()
    {
        yield return ValidateUser;
        yield return ValidateChannel;
    }

    private IEnumerable<ValidationResult> ValidateUser(TransactionRequest request, ValidationContext _)
    {
        var validState = Repository.User.ExistsUser(request.GuildId, request.MessageInfo.AuthorId) &&
                         (request.ReactionInfo == null || Repository.User.ExistsUser(request.GuildId, request.ReactionInfo.UserId));

        if (!validState)
            yield return new ValidationResult("UnknownUser");
    }

    private IEnumerable<ValidationResult> ValidateChannel(TransactionRequest request, ValidationContext _)
    {
        if (!string.IsNullOrEmpty(request.ChannelId) && !Repository.Channel.ExistsChannel(request.GuildId, request.ChannelId))
            yield return new ValidationResult("UnknownChannel");
    }
}

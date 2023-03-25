using System.ComponentModel.DataAnnotations;
using PointsService.Core.Repository;
using PointsService.Models;

namespace PointsService.Validation;

public class TransactionRequestValidator
{
    private PointsServiceRepository Repository { get; }

    public TransactionRequestValidator(PointsServiceRepository repository)
    {
        Repository = repository;
    }

    public IEnumerable<ValidationResult> Validate(TransactionRequest request)
    {
        var validState = Repository.User.ExistsUser(request.GuildId, request.MessageInfo.AuthorId) &&
                         (request.ReactionInfo == null || Repository.User.ExistsUser(request.GuildId, request.ReactionInfo.UserId));

        if (!validState)
            yield return new ValidationResult("UnknownUser");

        if (!Repository.Channel.ExistsChannel(request.GuildId, request.ChannelId))
            yield return new ValidationResult("UnknownChannel");
    }

    public IEnumerable<ValidationResult> Validate(AdminTransactionRequest request)
    {
        if (!Repository.User.ExistsUser(request.GuildId, request.UserId))
            yield return new ValidationResult("UnknownUser");
    }
}

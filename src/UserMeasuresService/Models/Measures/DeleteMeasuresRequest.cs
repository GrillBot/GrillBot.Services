using System.ComponentModel.DataAnnotations;

namespace UserMeasuresService.Models.Measures;

public class DeleteMeasuresRequest : IValidatableObject
{
    public Guid? Id { get; set; }
    public long? ExternalId { get; set; }

    [StringLength(64)]
    public string? ExternalIdType { get; set; }

    public DeleteMeasuresRequest()
    {
    }

    public DeleteMeasuresRequest(Guid? id, long? externalId, string? externalIdType)
    {
        Id = id;
        ExternalId = externalId;
        ExternalIdType = externalIdType;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Id is null && ExternalId is null)
            yield return new ValidationResult("Missing measure ID", new[] { nameof(Id), nameof(ExternalId) });

        if (!string.IsNullOrEmpty(ExternalIdType) && ExternalId is null)
            yield return new ValidationResult("Required ExternalId when ExternalIdType is filled.");

        if (ExternalId is not null && string.IsNullOrEmpty(ExternalIdType))
            yield return new ValidationResult("Required ExternalIdType when ExternalId is filled.");
    }

    public static DeleteMeasuresRequest FromInternalId(Guid id) => new(id, null, null);
    public static DeleteMeasuresRequest FromExternalSystem(long externalId, string externalIdType) => new(null, externalId, externalIdType);
}

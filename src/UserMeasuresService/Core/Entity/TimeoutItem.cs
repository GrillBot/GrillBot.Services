using Microsoft.EntityFrameworkCore;

namespace UserMeasuresService.Core.Entity;

[Index(nameof(ExternalId), IsUnique = true)]
public class TimeoutItem : UserMeasureBase
{
    public DateTime ValidTo { get; set; }
    public long ExternalId { get; set; }
}

using Microsoft.EntityFrameworkCore;

namespace UserMeasuresService.Core.Entity;

[Index(nameof(LogSetId), IsUnique = true)]
public class UnverifyItem : UserMeasureBase
{
    public DateTime ValidTo { get; set; }
    public long LogSetId { get; set; }
}

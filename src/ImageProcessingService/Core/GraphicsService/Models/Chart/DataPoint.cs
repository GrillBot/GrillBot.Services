namespace ImageProcessingService.Core.GraphicsService.Models.Chart;

public class DataPoint
{
    public string Label { get; set; } = null!;
    public int? Value { get; set; }

    public IEnumerable<object> GetAtomicValues()
    {
        yield return Label;

        if (Value is not null)
            yield return Value;
    }
}

namespace ImageProcessingService.Core.GraphicsService.Models.Chart;

public class Label
{
    public string Text { get; set; } = null!;
    public string Align { get; set; } = null!;
    public int Size { get; set; }
    public string Color { get; set; } = null!;
    public string Weight { get; set; } = null!;

    public IEnumerable<object> GetAtomicValues()
    {
        yield return Text;
        yield return Align;
        yield return Size;
        yield return Color;
        yield return Weight;
    }
}

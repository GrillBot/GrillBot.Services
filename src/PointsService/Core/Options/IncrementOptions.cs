namespace PointsService.Core.Options;

public class IncrementOptions
{
    public int Min { get; set; }
    public int Max { get; set; }
    public int Cooldown { get; set; }
    public Dictionary<string, object> Config { get; set; } = new();

    public TValue? GetConfigurationValue<TValue>(string key)
    {
        if (!Config.TryGetValue(key, out var value))
            return default;
        return (TValue)Convert.ChangeType(value, typeof(TValue));
    }
}

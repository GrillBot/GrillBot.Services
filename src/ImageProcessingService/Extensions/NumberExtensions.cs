using System.Text;

namespace ImageProcessingService.Extensions;

public static class NumberExtensions
{
    public static string ToStringWithSpaces(this int value, int digitCount = 3)
    {
        var builder = new StringBuilder(value.ToString());
        for (var i = builder.Length - digitCount; i >= 0; i -= digitCount)
            builder.Insert(i, ' ');

        return builder.ToString().Trim();
    }
}

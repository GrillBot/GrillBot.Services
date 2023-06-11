using GrillBot.Core.Models;

namespace AuditLogService.Core.Extensions;

public static class DiffExtensions
{
    public static Diff<TType>? NullIfEquals<TType>(this Diff<TType> diff)
    {
        if (typeof(TType) == typeof(byte[]))
            return diff is { Before: byte[] before, After: byte[] after } && before.SequenceEqual(after) ? null : diff;

        return EqualityComparer<TType>.Default.Equals(diff.Before, diff.After) ? null : diff;
    }
}

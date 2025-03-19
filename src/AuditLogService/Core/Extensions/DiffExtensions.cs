using GrillBot.Core.Models;

namespace AuditLogService.Core.Extensions;

public static class DiffExtensions
{
    public static Diff<TType>? NullIfEquals<TType>(this Diff<TType> diff)
    {
        if (typeof(TType) != typeof(byte[]))
            return EqualityComparer<TType>.Default.Equals(diff.Before, diff.After) ? null : diff;

        var before = diff.Before as byte[] ?? [];
        var after = diff.After as byte[] ?? [];
        return before.SequenceEqual(after) ? null : diff;
    }
}

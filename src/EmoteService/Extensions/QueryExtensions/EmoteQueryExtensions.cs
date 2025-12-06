using EmoteService.Core.Entity.ValueObjects;

namespace EmoteService.Extensions.QueryExtensions;

public static class EmoteQueryExtensions
{
    public static IQueryable<TEntity> WithEmoteQuery<TEntity>(this IQueryable<TEntity> query, Discord.Emote emote) where TEntity : EmoteValueObject
        => query.Where(o => o.EmoteId == emote.Id.ToString() && o.EmoteName == emote.Name && o.EmoteIsAnimated == emote.Animated);
}

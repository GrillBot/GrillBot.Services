using System.Linq.Expressions;

namespace EmoteService.Extensions.QueryExtensions;

public static class QueryableExtensions
{
    public static IOrderedQueryable<TEntity> CreateSortingOfParameter<TEntity, TParameter>(
        this IQueryable<TEntity> query,
        Expression<Func<TEntity, TParameter>> sortExpression,
        bool descending
    ) where TEntity : class => descending ? query.OrderByDescending(sortExpression) : query.OrderBy(sortExpression);
}

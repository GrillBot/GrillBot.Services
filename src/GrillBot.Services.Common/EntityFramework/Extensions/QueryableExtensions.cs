using System.Linq.Expressions;

namespace GrillBot.Services.Common.EntityFramework.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TEntity> WithSorting<TEntity>(
        this IQueryable<TEntity> query,
        Expression<Func<TEntity, object>>[] sortExpressions,
        bool descending
    )
    {
        if (sortExpressions.Length == 0)
            return query;

        var sortQuery = descending ?
            query.OrderByDescending(sortExpressions[0]) :
            query.OrderBy(sortExpressions[0]);

        foreach (var expression in sortExpressions.Skip(1))
        {
            sortQuery = descending ?
                sortQuery.ThenByDescending(expression) :
                sortQuery.ThenBy(expression);
        }

        return sortQuery;
    }
}

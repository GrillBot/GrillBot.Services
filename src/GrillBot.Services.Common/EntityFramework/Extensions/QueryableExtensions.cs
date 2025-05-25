using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

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

    public static async Task<int> CountAsync(this IQueryable query, CancellationToken cancellationToken = default)
    {
        var countAsyncMethod = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .First(m =>
                m.Name == nameof(EntityFrameworkQueryableExtensions.CountAsync) &&
                m.GetParameters().Length == 2 &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>)
            )
            .MakeGenericMethod(query.ElementType);

        return await (Task<int>)countAsyncMethod.Invoke(null, [query, cancellationToken])!;
    }
}

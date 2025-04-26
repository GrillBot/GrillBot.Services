using GrillBot.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace GrillBot.Services.Common.EntityFramework.Extensions;

public static class ServiceCollectionExtensions
{
    private const string SQL_RESTART_ERROR_CODE = "57P01";

    public static IServiceCollection AddPostgresDatabaseContext<TContext>(
        this IServiceCollection services,
        string connectionString,
        Func<DbContextOptionsBuilder, DbContextOptionsBuilder>? efBuilder = null,
        Action<NpgsqlDbContextOptionsBuilder>? pgBuilder = null
    ) where TContext : DbContext
    {
        return services.AddDatabaseContext<TContext>(b =>
        {
            b = b.UseNpgsql(
                connectionString,
                opt =>
                {
                    opt = opt.EnableRetryOnFailure([SQL_RESTART_ERROR_CODE]);
                    pgBuilder?.Invoke(opt);
                }
            );

            if (efBuilder is not null)
                b = efBuilder(b);

            return b;
        });
    }
}

using GrillBot.Core.Database;
using GrillBot.Services.Common.EntityFramework.Extensions;
using GrillBot.Services.Common.Telemetry.Database;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GrillBot.Services.Common.EntityFramework;

public class GrillBotServiceDbContext(
    DbContextOptions options,
    DatabaseTelemetryCollector _telemetryCollector
) : GrillBotBaseContext(options)
{
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges();

        var changedTypes = ChangeTracker.Entries()
            .Where(o => o.State is EntityState.Added or EntityState.Deleted or EntityState.Modified)
            .Select(o => new
            {
                Type = o.Entity.GetType(),
                Schema = o.Metadata.GetSchema() ?? o.Metadata.GetDefaultSchema(),
                TableName = o.Metadata.GetTableName()!
            })
            .DistinctBy(o => o.Type)
            .ToList();

        var changedRows = await base.SaveChangesAsync(cancellationToken);

        foreach (var entity in changedTypes)
        {
            var set = GetEntityQuery(entity.Type);
            var count = await set.CountAsync(cancellationToken);
            var entityName = string.IsNullOrEmpty(entity.Schema) ? entity.TableName : $"{entity.Schema}.{entity.TableName}";

            _telemetryCollector.SetTableCount(entityName, count);
        }

        return changedRows;
    }

    public async Task<Dictionary<string, long>> GetRecordsCountInTablesAsync(CancellationToken cancellationToken = default)
    {
        var result = new Dictionary<string, long>();

        foreach (var entityType in Model.GetEntityTypes())
        {
            var schema = entityType.GetSchema() ?? entityType.GetDefaultSchema();
            var tableName = entityType.GetTableName()!;
            var query = GetEntityQuery(entityType.ClrType);

            result.Add(
                string.IsNullOrEmpty(schema) ? tableName : $"{schema}.{tableName}",
                await query.CountAsync(cancellationToken)
            );
        }

        return result;
    }

    private IQueryable GetEntityQuery(Type entityType)
    {
        return (IQueryable)GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .First(m =>
                m.Name == nameof(Set) &&
                m.IsGenericMethod &&
                m.GetParameters().Length == 0
            )
            .MakeGenericMethod(entityType)
            .Invoke(this, null)!;
    }
}

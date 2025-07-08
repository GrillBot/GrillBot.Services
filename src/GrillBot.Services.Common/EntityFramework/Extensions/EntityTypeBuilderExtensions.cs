using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrillBot.Services.Common.EntityFramework.Extensions;

public static class EntityTypeBuilderExtensions
{
    public static void WithSchema<TEntity>(this EntityTypeBuilder<TEntity> builder, string schemaName) where TEntity : class
    {
        var tableName = builder.Metadata.GetTableName() ?? builder.Metadata.GetDefaultTableName();
        builder.ToTable(tableName!, schemaName);
    }
}

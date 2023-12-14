using System.Reflection;
using GrillBot.Core.Models;

namespace AuditLogService.Core.Helpers;

public static class ModelHelper
{
    public static bool IsModelEmpty<TModel>(TModel model, bool ignoreNonDiffTypes = true)
    {
        var modelType = typeof(TModel);
        var diffType = typeof(Diff<>);

        foreach (var property in modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (!property.PropertyType.IsGenericType)
            {
                if (ignoreNonDiffTypes)
                    continue;
                return false;
            }

            var genericType = property.PropertyType.GetGenericTypeDefinition();
            if (genericType != diffType)
            {
                if (ignoreNonDiffTypes)
                    continue;
                return false;
            }

            // If property contains value, return indication that model is not empty. Otherwise process to next property.
            if (property.GetValue(model, null) is not null)
                return false;
        }

        return true;
    }
}

using GrillBot.Core.Extensions;
using GrillBot.Core.Infrastructure.Actions;
using GrillBot.Services.Common.Infrastructure.Api;
using System.Globalization;
using System.Linq;
using System.Text.Json.Nodes;
using UnverifyService.Core.Entity;
using UnverifyService.Core.Entity.Logs;
using UnverifyService.Core.Enums;

namespace UnverifyService.Actions.Import;

public class ImportLegacyLogItemAction(IServiceProvider serviceProvider) : ApiAction<UnverifyContext>(serviceProvider)
{
    public override async Task<ApiResult> ProcessAsync()
    {
        var jsonData = GetParameter<JsonObject>(0);

        var entity = ParseRootEntity(jsonData);
        if (await ContextHelper.IsAnyAsync(DbContext.LogItems.Where(o => o.LogNumber == entity.LogNumber), CancellationToken))
            return ApiResult.Ok();

        var rawData = jsonData["Data"]?.GetValue<string>() ?? throw new InvalidDataException("Missing field 'Data' in the JSON.");
        var dataField = JsonNode.Parse(rawData)?.AsObject() ?? throw new InvalidDataException("Missing field 'Data' in the JSON.");

        switch (entity.OperationType)
        {
            case UnverifyOperationType.Unverify:
            case UnverifyOperationType.SelfUnverify:
                entity.SetOperation = ParseSetOperation(dataField);
                break;
        }

        await DbContext.AddAsync(entity, CancellationToken);
        await ContextHelper.SaveChangesAsync(CancellationToken);

        return ApiResult.Ok();
    }

    private static UnverifyLogItem ParseRootEntity(JsonObject jsonData)
    {
        var operation = ReadRequiredField<int>(jsonData, "Operation");

        return new UnverifyLogItem
        {
            Id = Guid.NewGuid(),
            CreatedAt = ReadRequiredField<DateTime>(jsonData, "CreatedAt").WithKind(DateTimeKind.Local).ToUniversalTime(),
            GuildId = ReadRequiredField<string>(jsonData, "GuildId").ToUlong(),
            FromUserId = ReadRequiredField<string>(jsonData, "FromUserId").ToUlong(),
            LogNumber = ReadRequiredField<long>(jsonData, "Id"),
            OperationType = operation switch
            {
                0 => UnverifyOperationType.Unverify,
                1 => UnverifyOperationType.SelfUnverify,
                2 => UnverifyOperationType.AutoRemove,
                3 => UnverifyOperationType.ManualRemove,
                4 => UnverifyOperationType.Update,
                5 => UnverifyOperationType.Recovery,
                _ => throw new InvalidDataException($"Incompatible operation type {operation}")
            },
            ToUserId = ReadRequiredField<string>(jsonData, "ToUserId").ToUlong()
        };
    }

    private static UnverifyLogSetOperation? ParseSetOperation(JsonObject jsonData)
    {
        var end = ReadRequiredField<DateTime>(jsonData, "End");
        if (end.Kind == DateTimeKind.Unspecified)
            end = end.WithKind(DateTimeKind.Local);

        var start = ReadRequiredField<DateTime>(jsonData, "Start");
        if (start.Kind == DateTimeKind.Unspecified)
            start = start.WithKind(DateTimeKind.Local);

        return new UnverifyLogSetOperation
        {
            EndAtUtc = end.ToUniversalTime(),
            KeepMutedRole = ReadOptionalField<bool>(jsonData, "KeepMutedRole"),
            Language = ReadOptionalField<string>(jsonData, "Language") ?? "en-US",
            Reason = ReadOptionalField<string>(jsonData, "Reason"),
            StartAtUtc = start.ToUniversalTime(),
            Channels = [
                .. (jsonData["ChannelsToKeep"]?.AsArray() ?? []).Select(node => new UnverifyLogSetChannel
                {
                    AllowValue = ReadRequiredField<ulong>(node!, "AllowValue"),
                    DenyValue = ReadRequiredField<ulong>(node!, "DenyValue"),
                    ChannelId = ReadRequiredField<ulong>(node!, "ChannelId"),
                    IsKept = true
                }),
                .. (jsonData["ChannelsToRemove"]?.AsArray() ?? []).Select(node => new UnverifyLogSetChannel
                {
                    AllowValue = ReadRequiredField<ulong>(node!, "AllowValue"),
                    DenyValue = ReadRequiredField<ulong>(node!, "DenyValue"),
                    ChannelId = ReadRequiredField<ulong>(node!, "ChannelId"),
                    IsKept = false
                })
            ],
            Roles = [
                .. (jsonData["RolesToKeep"]?.AsArray() ?? []).Select(node => new UnverifyLogSetRole
                {
                    IsKept = true,
                    RoleId = node!.GetValue<ulong>()
                }),
                .. (jsonData["RolesToRemove"]?.AsArray() ?? []).Select(node => new UnverifyLogSetRole
                {
                    IsKept = false,
                    RoleId = node!.GetValue<ulong>()
                })
            ]
        };
    }

    private static T ReadRequiredField<T>(JsonNode jsonData, string field)
    {
        var jsonField = jsonData[field] ?? throw new InvalidDataException($"Missing field '{field}' in the JSON data.");

        if (typeof(T) == typeof(DateTime))
        {
            var rawValue = jsonField.GetValue<string>();
            if (DateTime.TryParse(rawValue, new CultureInfo("cs-CZ"), out var dateTime))
                return (T)Convert.ChangeType(dateTime, typeof(T));

            if (DateTime.TryParse(rawValue, new CultureInfo("en-US"), out dateTime))
                return (T)Convert.ChangeType(dateTime, typeof(T));

            if (DateTime.TryParse(rawValue, CultureInfo.InvariantCulture, out dateTime))
                return (T)Convert.ChangeType(dateTime, typeof(T));
        }

        return jsonField.GetValue<T>();
    }

    private static T? ReadOptionalField<T>(JsonNode jsonData, string field)
    {
        var jsonField = jsonData[field];
        if (jsonField is null)
            return default;

        if (typeof(T) == typeof(DateTime))
        {
            var rawValue = jsonField.GetValue<string>();
            if (DateTime.TryParse(rawValue, new CultureInfo("cs-CZ"), out var dateTime))
                return (T)Convert.ChangeType(dateTime, typeof(T));

            if (DateTime.TryParse(rawValue, new CultureInfo("en-US"), out dateTime))
                return (T)Convert.ChangeType(dateTime, typeof(T));

            if (DateTime.TryParse(rawValue, CultureInfo.InvariantCulture, out dateTime))
                return (T)Convert.ChangeType(dateTime, typeof(T));
        }

        if (typeof(T) == typeof(string))
        {
            var rawValue = jsonField.GetValue<string>();
            return string.IsNullOrEmpty(rawValue) ? default : (T?)Convert.ChangeType(rawValue, typeof(T));
        }

        return jsonField.GetValue<T>();
    }
}

﻿using GrillBot.Services.Common.Telemetry;
using UnverifyService.Actions.Archivation;

namespace UnverifyService.Telemetry.Initializers;

public class ArchivationInitializer(
    IServiceProvider serviceProvider,
    UnverifyTelemetryCollector _collector
) : TelemetryInitializerBase(serviceProvider)
{
    protected override async Task ExecuteInternalAsync(IServiceProvider provider, CancellationToken cancellationToken = default)
    {
        var action = provider.GetRequiredService<CreateArchivationDataAction>();
        action.SetCancellationToken(cancellationToken);

        _collector.ItemsToArchive.Set(await action.CountAsync());
    }
}

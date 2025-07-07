using GrillBot.Services.Common;
using GrillBot.Services.Common.EntityFramework.Extensions;
using System.Reflection;
using UnverifyService.Options;

var application = await ServiceBuilder.CreateWebAppAsync<AppOptions>(
    Assembly.GetExecutingAssembly(),
    args,
    (services, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;

    },
    configureHealthChecks: (healthCheckBuilder, configuration) =>
    {
        var connectionString = configuration.GetConnectionString("Default")!;
        healthCheckBuilder.AddNpgSql(connectionString);
    }
);

await application.RunAsync();

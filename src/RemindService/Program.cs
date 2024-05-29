using GrillBot.Services.Common;
using RemindService.Options;
using System.Reflection;

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
    },
    preRunInitialization: (app, _) => Task.CompletedTask
);

await application.RunAsync();
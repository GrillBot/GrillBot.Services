using GrillBot.Services.Common;
using System.Reflection;
using UserManagementService.Options;

var application = await ServiceBuilder.CreateWebAppAsync<AppOptions>(
    Assembly.GetExecutingAssembly(),
    args,
    (services, configuration) =>
    {
    },
    configureHealthChecks: (healthCheckBuilder, configuration) =>
    {
    },
    preRunInitialization: (app, _) => Task.CompletedTask
);

await application.RunAsync();

using GrillBot.Core;
using Microsoft.AspNetCore.HttpOverrides;
using RubbergodService.Core;
using RubbergodService.Core.Entity;
using RubbergodService.Discord;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services
    .AddDatabase(builder.Configuration, out var connectionString)
    .AddManagers()
    .AddDiscord()
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddDirectApi()
    .AddMemberSync()
    .AddMemoryCache()
    .AddDiagnostic()
    .AddCoreManagers()
    .AddStatisticsProvider<StatisticsProvider>();
builder.Services.AddControllers(c => c.RegisterCoreFilter());

builder.Services
    .AddHealthChecks()
    .AddNpgSql(connectionString);
builder.Services.Configure<ForwardedHeadersOptions>(opt => opt.ForwardedHeaders = ForwardedHeaders.All);

var app = builder.Build();

app.Services.GetRequiredService<DiscordLogManager>();
await app.InitDatabaseAsync<RubbergodServiceContext>();

using var scope = app.Services.CreateScope();
await scope.ServiceProvider.GetRequiredService<DiscordManager>().LoginAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

using AuditLogService.Core;
using AuditLogService.Core.Discord;
using AuditLogService.Core.Entity;
using GrillBot.Core;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(opt => opt.AddServerHeader = false);
builder.Services.AddCoreServices(builder.Configuration);

var app = builder.Build();

app.Services.GetRequiredService<DiscordLogManager>();
await app.InitDatabaseAsync<AuditLogServiceContext>();

using var scope = app.Services.CreateScope();
await scope.ServiceProvider.GetRequiredService<DiscordManager>().LoginAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

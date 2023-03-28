using GrillBot.Core;
using RubbergodService.Core;
using RubbergodService.Core.Entity;
using RubbergodService.Discord;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCoreServices(builder.Configuration);

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

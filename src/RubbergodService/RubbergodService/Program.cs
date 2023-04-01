using GrillBot.Core;
using RubbergodService.Core;
using RubbergodService.Core.Entity;
using RubbergodService.Discord;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(opt => opt.AddServerHeader = false);
builder.Services.AddCoreServices(builder.Configuration);

var app = builder.Build();

app.Services.GetRequiredService<DiscordLogManager>();
await app.InitDatabaseAsync<RubbergodServiceContext>();

using var scope = app.Services.CreateScope();
await scope.ServiceProvider.GetRequiredService<DiscordManager>().LoginAsync();

app.Use((context, next) =>
{
    context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

    return next();
});

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

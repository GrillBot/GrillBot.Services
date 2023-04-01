using GrillBot.Core;
using PointsService.Core;
using PointsService.Core.Entity;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(opt => opt.AddServerHeader = false);
builder.Services.AddCoreServices(builder.Configuration);

var app = builder.Build();
await app.InitDatabaseAsync<PointsServiceContext>();

app.Use((context, next) =>
{
    context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
    context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

    return next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

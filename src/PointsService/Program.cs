using Microsoft.EntityFrameworkCore;
using PointsService.Core;
using PointsService.Core.Entity;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCoreServices(builder.Configuration);

var app = builder.Build();

using var scope = app.Services.CreateScope();
await scope.ServiceProvider.GetRequiredService<PointsServiceContext>().Database.MigrateAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

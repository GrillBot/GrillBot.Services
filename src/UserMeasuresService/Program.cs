using GrillBot.Core;
using UserMeasuresService.Core;
using UserMeasuresService.Core.Entity;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(opt => opt.AddServerHeader = false);
builder.Services.AddCoreServices(builder.Configuration);

var app = builder.Build();

await app.InitDatabaseAsync<UserMeasuresContext>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

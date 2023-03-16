using Discord;
using FileService.Cache;
using FileService.Managers;
using GrillBot.Core;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1073741824; // 1GB 
});

builder.Services.AddScoped<StorageManager>();
builder.Services.AddScoped<StorageCacheManager>();
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddControllers(c => c.RegisterCoreFilter());
builder.Services.AddDiagnostic();
builder.Services.AddSingleton<IDiscordClient>(_ => null!);
builder.Services.AddCoreManagers();
builder.Services.Configure<ForwardedHeadersOptions>(opt => opt.ForwardedHeaders = ForwardedHeaders.All);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

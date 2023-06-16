using Azure.Storage.Blobs;
using FileService.Actions;
using FileService.Cache;
using FileService.Factory;
using GrillBot.Core;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1073741824; // 1GB
    options.AddServerHeader = false;
});

builder.Services.AddScoped<StorageCacheManager>();
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddControllers(c => c.RegisterCoreFilter());
builder.Services.AddDiagnostic();
builder.Services.AddCoreManagers();
builder.Services.AddActions();
builder.Services.AddScoped<BlobContainerFactory>();
builder.Services.AddScoped<BlobContainerClient>(provider => provider.GetRequiredService<BlobContainerFactory>().CreateClient());

builder.Services.Configure<RouteOptions>(opt => opt.LowercaseUrls = true);
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

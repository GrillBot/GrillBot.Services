namespace FileService.Actions;

public static class ActionExtensions
{
    public static IServiceCollection AddActions(this IServiceCollection services)
    {
        return services
            .AddScoped<UploadFileAction>()
            .AddScoped<DownloadFileAction>()
            .AddScoped<GenerateLinkAction>()
            .AddScoped<DeleteFileAction>();
    }
}

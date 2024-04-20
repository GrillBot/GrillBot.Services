using Microsoft.Extensions.DependencyInjection;

namespace GrillBot.Services.Common.Registrators;

public static class DiagnosticsRegistrator
{
    public static void RegisterCommonControllers(this IMvcBuilder mvc)
    {
        mvc.AddApplicationPart(typeof(DiagnosticsRegistrator).Assembly);
    }
}

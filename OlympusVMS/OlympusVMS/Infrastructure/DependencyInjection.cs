using OlympusVMS.Data;
using OlympusVMS.Services;

namespace OlympusVMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddOlympusVms(this IServiceCollection services)
    {
        services.AddRazorComponents()
            .AddInteractiveServerComponents();
        
        services.AddOlympusServices();
        services.AddOlympusRepositories();
        
        return services;
    }
}
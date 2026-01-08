using Microsoft.Extensions.DependencyInjection;
using OlympusVMS.Services.Interfaces;
using OlympusVMS.Services.Repositories.MeetingRepository;
using OlympusVMS.Services.Services;

namespace OlympusVMS.Services;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers OlympusVMS service-layer dependencies.
    /// Keep Program.cs clean by calling: builder.Services.AddOlympusServices();
    /// </summary>
    public static IServiceCollection AddOlympusServices(
        this IServiceCollection services)
    {
        services.AddScoped<IMeetingService, MeetingService>();

        return services;
    }

    public static IServiceCollection AddOlympusRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IMeetingRepository, MeetingRepository>();
        
        return services;
    }
}
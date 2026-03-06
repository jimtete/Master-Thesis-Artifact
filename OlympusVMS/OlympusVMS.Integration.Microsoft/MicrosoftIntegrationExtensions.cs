using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using OlympusVMS.Integration.Microsoft.Services;

namespace OlympusVMS.Integration.Microsoft
{
    public static class MicrosoftIntegrationExtensions
    {
        public static IServiceCollection AddMicrosoftGraphIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            var initialScopes = configuration.GetValue<string>("MicrosoftGraph:Scopes")?.Split(' ');

            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(configuration.GetSection("AzureAd"))
                .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
                .AddMicrosoftGraph(configuration.GetSection("MicrosoftGraph"))
                .AddInMemoryTokenCaches();

            services.AddScoped<IMicrosoftCalendarService, MicrosoftCalendarService>();

            return services;
        }
    }
}

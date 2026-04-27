using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using OlympusVMS.Integration.Microsoft.Configuration;
using OlympusVMS.Integration.Microsoft.Services;
using System.Security.Claims;

namespace OlympusVMS.Integration.Microsoft
{
    public static class MicrosoftIntegrationExtensions
    {
        public static IServiceCollection AddMicrosoftGraphIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            var initialScopes = configuration.GetValue<string>("MicrosoftGraph:Scopes")?.Split(' ');

            services.AddMemoryCache();

            services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(configuration.GetSection("AzureAd"))
                .EnableTokenAcquisitionToCallDownstreamApi(initialScopes)
                .AddMicrosoftGraph(configuration.GetSection("MicrosoftGraph"))
                .AddInMemoryTokenCaches();

            services.AddOptions<MeetingRoomOptions>()
                .Bind(configuration.GetSection(MeetingRoomOptions.SectionName));

            services.AddScoped<IMicrosoftCalendarService, MicrosoftCalendarService>();
            services.AddHttpContextAccessor();

            return services;
        }
    }
}

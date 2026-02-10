using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using OlympusVMS.Integration.Microsoft.Services;
using OlympusVMS.Utils.Configuration;

namespace OlympusVMS.Integration.Microsoft
{
    public static class MicrosoftIntegrationExtensions
    {
        public static IServiceCollection AddMicrosoftGraphIntegration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MicrosoftGraphOptions>(
                configuration.GetSection(MicrosoftGraphOptions.SectionName));

            services.AddSingleton(sp =>
            {
                var options = sp.GetRequiredService<IOptions<MicrosoftGraphOptions>>().Value;
                var logger = sp.GetRequiredService<ILogger<KeyVaultCertificateService>>();

                var keyVaultUri = "https://kv-cfrms-prod.vault.azure.net/";
                var certService = new KeyVaultCertificateService(logger, keyVaultUri);

                var certificate = certService.GetCertificateAsync(options.CertificateName)
                    .GetAwaiter().GetResult();

                var credential = new ClientCertificateCredential(
                        options.TenantId,
                        options.ClientId,
                        certificate
                    );

                return new GraphServiceClient(credential);
            });

            services.AddScoped<IMicrosoftCalendarService, MicrosoftCalendarService>();

            return services;
        }
    }
}

using Azure.Identity;
using Azure.Security.KeyVault.Certificates;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography.X509Certificates;

namespace OlympusVMS.Integration.Microsoft
{
    public class KeyVaultCertificateService
    {
        private readonly ILogger<KeyVaultCertificateService> _logger;
        private readonly CertificateClient _certificateClient;

        public KeyVaultCertificateService(ILogger<KeyVaultCertificateService> logger, string keyVaultUri)
        {
            _logger = logger;
            _certificateClient = new CertificateClient(new Uri(keyVaultUri), new DefaultAzureCredential());
        }

        public async Task<X509Certificate2> GetCertificateAsync(string certificateName)
        {
            if (string.IsNullOrWhiteSpace(certificateName))
            {
                throw new ArgumentException("Certificate name is required");
            }

            try
            {
                _logger.LogInformation("Retrieving certificate '{CertificateName}' from Key Vault", certificateName);

                var response = await _certificateClient.DownloadCertificateAsync(certificateName);

                return response.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve certificate '{CertificateName}' from Key Vault", certificateName);
                throw;
            }
        }
    }
}

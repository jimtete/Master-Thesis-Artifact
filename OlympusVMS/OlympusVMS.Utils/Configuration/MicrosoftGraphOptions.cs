namespace OlympusVMS.Utils.Configuration
{
    public class MicrosoftGraphOptions
    {
        public const string SectionName = "MicrosoftGraph";

        public string TenantId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string CertificateName { get; set; } = string.Empty;
    }
}

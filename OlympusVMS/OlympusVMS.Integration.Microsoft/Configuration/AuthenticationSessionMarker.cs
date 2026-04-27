namespace OlympusVMS.Integration.Microsoft.Configuration
{
    public sealed class AuthenticationSessionMarker
    {
        public string Value { get; } = Guid.NewGuid().ToString("N");
    }
}
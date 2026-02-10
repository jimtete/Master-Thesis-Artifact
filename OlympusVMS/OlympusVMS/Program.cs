using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OlympusVMS.Data;
using OlympusVMS.Infrastructure;
using OlympusVMS.Integration.Microsoft;
using OlympusVMS.Integration.Microsoft.Services;
using OlympusVMS.Utils.Configuration;

var builder = WebApplication.CreateBuilder(args);

// --- DATABASE BYPASS SECTION ---
// We keep the Options registration so the code doesn't crash if something expects it,
// but we comment out the actual DbContext and HealthCheck connection logic.

builder.Services.AddControllers();

builder.Services
    .AddOptions<DatabaseOptions>()
    .Bind(builder.Configuration.GetSection(DatabaseOptions.SectionName));
// .ValidateOnStart(); // Commented out to prevent startup crash without DB

/* builder.Services.AddDbContext<OlympusContext>((sp, options) =>
{
    var dbOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
    options.UseSqlServer(dbOptions.ConnectionString);
});
*/

/*
builder.Services.AddHealthChecks()
    .AddDbContextCheck<OlympusContext>("db");
*/
// -------------------------------

//builder.Services.AddOlympusVms();
builder.Services.AddMicrosoftGraphIntegration(builder.Configuration);

var app = builder.Build();

app.Logger.LogInformation("Environment: {Env}", app.Environment.EnvironmentName);

// --- TEST FETCH LOGIC ---
using (var scope = app.Services.CreateScope())
{
    var calendarService = scope.ServiceProvider.GetRequiredService<IMicrosoftCalendarService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation(">>> STARTING MS GRAPH SMOKE TEST <<<");

        // NEW: Get the options to find the cert name
        var options = scope.ServiceProvider.GetRequiredService<IOptions<MicrosoftGraphOptions>>().Value;

        // NEW: Check the certificate thumbprint before the call fails
        // This helps verify if the App Registration actually has this specific cert registered.
        var keyVaultUri = "https://kv-cfrms-prod.vault.azure.net/";
        var certService = new KeyVaultCertificateService(scope.ServiceProvider.GetRequiredService<ILogger<KeyVaultCertificateService>>(), keyVaultUri);
        var cert = await certService.GetCertificateAsync(options.CertificateName);

        logger.LogInformation("USING CERTIFICATE: {Subject}", cert.Subject);
        logger.LogInformation("CERTIFICATE THUMBPRINT: {Thumbprint}", cert.Thumbprint);

        string testEmail = "dimitrios.tetepoulidis@capital-four.com";
        var meetings = await calendarService.GetUserMeetingsAsync(testEmail);

        logger.LogInformation("SUCCESS: Found {Count} meetings.", meetings.Count);

        foreach (var meeting in meetings)
        {
            logger.LogInformation("Meeting: {Subject} | Start: {Start}",
                meeting.Subject,
                meeting.Start?.ToString("yyyy-MM-dd HH:mm zzz"));
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, ">>> MS GRAPH TEST FAILED! <<<");
        // Log inner exception as well, as Azure.Identity errors are often nested
        if (ex.InnerException != null)
            logger.LogError("Inner Error: {Message}", ex.InnerException.Message);
    }
}
// -----------------------

// Optional: Comment out Run() if you just want it to execute the test and stop
// app.Run();
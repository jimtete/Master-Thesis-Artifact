using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using OlympusVMS.Integration.Microsoft;
using OlympusVMS.Services.Interfaces;
using OlympusVMS.Services.Repositories.MeetingRepository;
using OlympusVMS.Services.Services;
using OlympusVMS.Utils.Configuration;

//var builder = WebApplication.CreateBuilder(args);

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    // This tells .NET: "Stop checking my dependencies on startup, I know what I'm doing!"
    EnvironmentName = Environments.Development
});

// --- 1. ENABLE BLAZOR & LOGIN UI ---
// These are required to handle the web redirect and "Access Denied" consent screens
builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddMicrosoftIdentityConsentHandler();

// --- 2. DATABASE BYPASS SECTION ---
builder.Services
    .AddOptions<DatabaseOptions>()
    .Bind(builder.Configuration.GetSection(DatabaseOptions.SectionName));

// --- 3. MICROSOFT GRAPH REGISTRATION ---
// This uses the code you wrote in MicrosoftIntegrationExtensions.cs
// It handles the Key Vault Cert + Delegated User Token automatically.
builder.Services.AddMicrosoftGraphIntegration(builder.Configuration);

//builder.Services.AddScoped<IMeetingService, MeetingService>();
//builder.Services.AddScoped<IMeetingRepository, MeetingRepository>();
builder.WebHost.UseUrls("https://localhost:5001");

var app = builder.Build();

app.Logger.LogInformation("Environment: {Env}", app.Environment.EnvironmentName);

// --- 4. MIDDLEWARE PIPELINE ---
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// CRITICAL: These two lines are what actually trigger the Delegated login flow
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapControllers();
app.MapBlazorHub();
app.MapRazorComponents<OlympusVMS.Components.App>()
    .AddInteractiveServerRenderMode();

app.Logger.LogInformation(">>> APP STARTED. OPEN BROWSER TO TEST DELEGATED GRAPH FLOW <<<");

app.Run();
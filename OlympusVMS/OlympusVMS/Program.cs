using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using OlympusVMS.Integration.Microsoft;
using OlympusVMS.Utils.Configuration;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    EnvironmentName = Environments.Development
});

builder.Services.AddControllersWithViews().AddMicrosoftIdentityUI();
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddMicrosoftIdentityConsentHandler();

builder.Services.AddAuthorizationBuilder();

builder.Services
    .AddOptions<DatabaseOptions>()
    .Bind(builder.Configuration.GetSection(DatabaseOptions.SectionName));

builder.Services.AddMicrosoftGraphIntegration(builder.Configuration);

builder.WebHost.UseUrls("https://localhost:5001");

var app = builder.Build();

app.Logger.LogInformation("Environment: {Env}", app.Environment.EnvironmentName);

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapControllers();
app.MapBlazorHub();
app.MapRazorComponents<OlympusVMS.Components.App>()
    .AddInteractiveServerRenderMode()
    .RequireAuthorization();

app.Logger.LogInformation(">>> APP STARTED. OPEN BROWSER TO TEST DELEGATED GRAPH FLOW <<<");

app.Run();
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OlympusVMS.Data;
using OlympusVMS.Infrastructure;
using OlympusVMS.Utils.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddOptions<DatabaseOptions>()
    .Bind(builder.Configuration.GetSection(DatabaseOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddDbContext<OlympusContext>((sp, options) =>
{
    var dbOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
    options.UseSqlServer(dbOptions.ConnectionString);
});
    

builder.Services.AddHealthChecks()
    .AddDbContextCheck<OlympusContext>("db");

builder.Services.AddOlympusVms();

var app = builder.Build();
app.Logger.LogInformation("Environment: {Env}",  app.Environment.EnvironmentName);

app.UseOlympusVmsPipeline();
app.MapHealthChecks("/health");

app.Run();
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using OlympusVMS.Data;
using OlympusVMS.Services;
using OlympusVMS.Utils.Configuration;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory,
});

Console.WriteLine($"ENV = {builder.Environment.EnvironmentName}");
Console.WriteLine($"ContentRoot = {builder.Environment.ContentRootPath}");
Console.WriteLine($"DB = '{builder.Configuration["Database:ConnectionString"]}'");


builder.Services.AddControllers();

builder.Services.AddOptions<DatabaseOptions>()
    .Bind(builder.Configuration.GetSection(DatabaseOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddDbContext<OlympusContext>((sp, options) =>
{
    var dbOptions = sp.GetRequiredService<IOptions<DatabaseOptions>>().Value;
    options.UseSqlServer(dbOptions.ConnectionString);
    options.UseSqlServer(dbOptions.ConnectionString,
        sql => sql.MigrationsAssembly(typeof(OlympusContext).Assembly.FullName));
});

// Register Repositories
builder.Services.AddOlympusRepositories();
builder.Services.AddOlympusServices();

// Swagger (UI)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OlympusVMS Integration API",
        Version = "v1"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "OlympusVMS Integration API v1");
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// test endpoint
app.MapControllers();
app.MapGet("/health", () => Results.Ok("OK"));

app.Run();
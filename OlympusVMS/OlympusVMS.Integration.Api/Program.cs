using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

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
        // optional: make Swagger the root page
        // c.RoutePrefix = string.Empty;
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// test endpoint
app.MapGet("/health", () => Results.Ok("OK"));

app.Run();
using OlympusVMS.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOlympusVms();

var app = builder.Build();

app.UseOlympusVmsPipeline();

app.Run();
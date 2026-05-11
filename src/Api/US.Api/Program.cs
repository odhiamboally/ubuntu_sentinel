using System.Text.Json.Serialization;
using US.Api.Features.Accountability;
using US.Api.Features.Intake;
using US.Api.Features.Regions;
using US.Api.Features.Reports;
using US.Api.Features.Realtime;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorClient", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5009",
                "https://localhost:7128",
                "http://127.0.0.1:5009",
                "https://127.0.0.1:7128")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
builder.Services.AddSingleton<IReportStore, InMemoryReportStore>();
builder.Services.AddSingleton<IRegionProfileStore, RegionProfileStore>();
builder.Services.AddSingleton<IPolicyComparisonService, SeededPolicyComparisonService>();
builder.Services.AddSingleton<IAccountabilityPipeline, DeterministicAccountabilityPipeline>();
builder.Services.AddSingleton<UssdSessionStore>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("BlazorClient");

app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Ok(new
{
    name = "Ubuntu Sentinel API",
    status = "running",
    endpoints = new[]
    {
        "/api/regions",
        "/api/reports",
        "/api/intake/ussd",
        "/api/reports/{id}/pipeline",
        "/hubs/reports"
    }
}));
app.MapReportEndpoints();
app.MapRegionEndpoints();
app.MapAccountabilityEndpoints();
app.MapUssdEndpoints();
app.MapHub<ReportHub>("/hubs/reports");

app.Run();

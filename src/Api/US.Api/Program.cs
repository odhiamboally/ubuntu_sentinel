using System.Text.Json.Serialization;
using QuestPDF.Infrastructure;
using US.Api.Features.Accountability;
using US.Api.Features.Auth;
using US.Api.Features.Intake;
using US.Api.Features.Regions;
using US.Api.Features.Reports;
using US.Api.Features.Realtime;
using US.SharedKernel.Inference;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;

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
builder.Services.AddSingleton<IssueTypeInferenceService>();
builder.Services.AddSingleton<IRegionProfileStore, RegionProfileStore>();
builder.Services.AddSingleton<SeededPolicyComparisonService>();
builder.Services.AddSingleton<IPolicyComparisonService>(provider => provider.GetRequiredService<SeededPolicyComparisonService>());
builder.Services.Configure<OpenAiPipelineOptions>(options =>
{
    options.ApiKey = builder.Configuration["OpenAI:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY");
    options.Model = builder.Configuration["OpenAI:Model"] ?? Environment.GetEnvironmentVariable("OPENAI_MODEL") ?? "gpt-4.1-mini";

    var timeoutRaw = builder.Configuration["OpenAI:RequestTimeoutSeconds"] ?? Environment.GetEnvironmentVariable("OPENAI_REQUEST_TIMEOUT_SECONDS");
    if (int.TryParse(timeoutRaw, out var timeoutSeconds))
    {
        options.RequestTimeoutSeconds = timeoutSeconds;
    }

    var retriesRaw = builder.Configuration["OpenAI:MaxRetries"] ?? Environment.GetEnvironmentVariable("OPENAI_MAX_RETRIES");
    if (int.TryParse(retriesRaw, out var maxRetries))
    {
        options.MaxRetries = maxRetries;
    }

    var retryDelayRaw = builder.Configuration["OpenAI:RetryBaseDelayMilliseconds"] ?? Environment.GetEnvironmentVariable("OPENAI_RETRY_BASE_DELAY_MS");
    if (int.TryParse(retryDelayRaw, out var retryDelayMilliseconds))
    {
        options.RetryBaseDelayMilliseconds = retryDelayMilliseconds;
    }
});
builder.Services.AddHttpClient<OpenAiChatJsonClient>();
builder.Services.AddSingleton<DeterministicAccountabilityPipeline>();
builder.Services.AddSingleton<IAccountabilityPipeline, OpenAiAccountabilityPipeline>();
builder.Services.AddSingleton<IBriefPdfRenderer, QuestPdfBriefRenderer>();
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
        "/api/reports/{id}/brief.pdf",
        "/api/policy-documents",
        "/hubs/reports"
    }
}));
app.MapGet("/api/policy-documents", (SeededPolicyComparisonService policyDocuments) => Results.Ok(policyDocuments.GetDocuments()))
    .WithTags("Policy Documents");
app.MapReportEndpoints();
app.MapRegionEndpoints();
app.MapAccountabilityEndpoints();
app.MapUssdEndpoints();
app.MapAuthEndpoints();
app.MapHub<ReportHub>("/hubs/reports");

app.Run();

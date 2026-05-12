using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using US.SharedKernel.Inference;
using US.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var hostBase = new Uri(builder.HostEnvironment.BaseAddress);
var apiBaseUrl = hostBase.Scheme == Uri.UriSchemeHttps
    ? builder.Configuration["HttpsApiBaseUrl"] ?? "https://localhost:7142"
    : builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5138";
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
builder.Services.AddScoped<UbuntuSentinelApiClient>();
builder.Services.AddScoped<OfflineReportQueue>();
builder.Services.AddScoped<AppLanguageService>();
builder.Services.AddSingleton<UiTextService>();
builder.Services.AddSingleton<IssueTypeInferenceService>();
builder.Services.AddMudServices();

await builder.Build().RunAsync();

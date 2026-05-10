using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using US.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5138";
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
builder.Services.AddScoped<UbuntuSentinelApiClient>();
builder.Services.AddScoped<OfflineReportQueue>();
builder.Services.AddSingleton<UiTextService>();
builder.Services.AddMudServices();

await builder.Build().RunAsync();

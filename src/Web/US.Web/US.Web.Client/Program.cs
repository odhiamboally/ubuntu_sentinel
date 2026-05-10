using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using US.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "http://localhost:5138";
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });
builder.Services.AddScoped<UbuntuSentinelApiClient>();

await builder.Build().RunAsync();

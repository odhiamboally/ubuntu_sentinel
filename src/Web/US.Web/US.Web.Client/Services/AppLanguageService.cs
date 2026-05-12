using Microsoft.JSInterop;

namespace US.Web.Client.Services;

public sealed class AppLanguageService
{
    private const string StorageKey = "ubuntu-sentinel-language";
    private bool _initialized;

    public string CurrentLanguage { get; private set; } = "en";

    public event Action? LanguageChanged;

    public async Task InitializeAsync(IJSRuntime jsRuntime)
    {
        if (_initialized)
        {
            return;
        }

        _initialized = true;

        try
        {
            var stored = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", StorageKey);
            CurrentLanguage = Normalize(stored);
        }
        catch
        {
            CurrentLanguage = "en";
        }
    }

    public async Task SetLanguageAsync(string languageCode, IJSRuntime jsRuntime)
    {
        var normalized = Normalize(languageCode);
        if (string.Equals(CurrentLanguage, normalized, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        CurrentLanguage = normalized;

        try
        {
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, normalized);
        }
        catch
        {
            // Runtime persistence is nice-to-have; in-memory state still updates the app.
        }

        LanguageChanged?.Invoke();
    }

    private static string Normalize(string? languageCode)
    {
        return string.Equals(languageCode, "fr", StringComparison.OrdinalIgnoreCase) ? "fr" : "en";
    }
}

using Microsoft.JSInterop;

namespace US.Web.Client.Services;

public enum DemoRole
{
    Reporter,
    Validator,
    Admin
}

public sealed class AppRoleService
{
    private const string StorageKey = "ubuntu-sentinel-role";
    private bool _initialized;

    public bool IsInitialized => _initialized;
    public DemoRole CurrentRole { get; private set; } = DemoRole.Reporter;
    public string? Username { get; private set; }
    public string? DisplayName { get; private set; }

    public event Action? RoleChanged;

    public bool IsReporter => CurrentRole == DemoRole.Reporter;
    public bool IsValidator => CurrentRole == DemoRole.Validator;
    public bool IsAdmin => CurrentRole == DemoRole.Admin;

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
            CurrentRole = Normalize(stored);
            Username = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", $"{StorageKey}-username");
            DisplayName = await jsRuntime.InvokeAsync<string?>("localStorage.getItem", $"{StorageKey}-display");
        }
        catch
        {
            CurrentRole = DemoRole.Reporter;
        }
    }

    public async Task SetAuthenticatedUserAsync(DemoRole role, string username, string displayName, IJSRuntime jsRuntime)
    {
        CurrentRole = role;
        Username = username;
        DisplayName = displayName;

        try
        {
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", StorageKey, role.ToString());
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", $"{StorageKey}-username", username);
            await jsRuntime.InvokeVoidAsync("localStorage.setItem", $"{StorageKey}-display", displayName);
        }
        catch
        {
            // Demo role persistence is best-effort only.
        }

        RoleChanged?.Invoke();
    }

    public Task LogoutAsync(IJSRuntime jsRuntime)
    {
        CurrentRole = DemoRole.Reporter;
        Username = null;
        DisplayName = null;

        try
        {
            return ClearAndNotifyAsync(jsRuntime);
        }
        catch
        {
            RoleChanged?.Invoke();
            return Task.CompletedTask;
        }
    }

    private async Task ClearAndNotifyAsync(IJSRuntime jsRuntime)
    {
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", StorageKey);
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", $"{StorageKey}-username");
        await jsRuntime.InvokeVoidAsync("localStorage.removeItem", $"{StorageKey}-display");
        RoleChanged?.Invoke();
    }

    private static DemoRole Normalize(string? role)
    {
        return Enum.TryParse<DemoRole>(role, ignoreCase: true, out var parsed)
            ? parsed
            : DemoRole.Reporter;
    }
}

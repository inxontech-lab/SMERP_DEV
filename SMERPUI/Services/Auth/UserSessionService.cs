using Microsoft.JSInterop;

namespace SMERPUI.Services.Auth;

public interface IUserSessionService
{
    Task<UserSession?> GetSessionAsync();
    Task<bool> IsLoggedInAsync();
    Task SetSessionAsync(UserSession session);
    Task ClearSessionAsync();
}

public class UserSessionService(IJSRuntime jsRuntime) : IUserSessionService
{
    private const string SessionStorageKey = "smerp-user-session";
    private UserSession? _cachedSession;
    private bool _isLoaded;

    public async Task<UserSession?> GetSessionAsync()
    {
        if (_isLoaded)
        {
            return _cachedSession;
        }

        _cachedSession = await jsRuntime.InvokeAsync<UserSession?>("sessionManager.get", SessionStorageKey);
        _isLoaded = true;

        return _cachedSession;
    }

    public async Task<bool> IsLoggedInAsync() => await GetSessionAsync() is not null;

    public async Task SetSessionAsync(UserSession session)
    {
        _cachedSession = session;
        _isLoaded = true;
        await jsRuntime.InvokeVoidAsync("sessionManager.set", SessionStorageKey, session);
    }

    public async Task ClearSessionAsync()
    {
        _cachedSession = null;
        _isLoaded = true;
        await jsRuntime.InvokeVoidAsync("sessionManager.remove", SessionStorageKey);
    }
}

using Microsoft.JSInterop;
using SMERPWeb.Services.Interop;

namespace SMERPWeb.Services.Auth;

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

        try
        {
            _cachedSession = await jsRuntime.InvokeAsync<UserSession?>("sessionManager.get", SessionStorageKey);
            _isLoaded = true;
        }
        catch (Exception ex) when (JsInteropGuard.IsUnavailable(ex))
        {
            return _cachedSession;
        }

        return _cachedSession;
    }

    public async Task<bool> IsLoggedInAsync() => await GetSessionAsync() is not null;

    public async Task SetSessionAsync(UserSession session)
    {
        _cachedSession = session;
        _isLoaded = true;

        try
        {
            await jsRuntime.InvokeVoidAsync("sessionManager.set", SessionStorageKey, session);
        }
        catch (Exception ex) when (JsInteropGuard.IsUnavailable(ex))
        {
            // During static pre-render we only keep the in-memory cache.
        }
    }

    public async Task ClearSessionAsync()
    {
        _cachedSession = null;
        _isLoaded = true;

        try
        {
            await jsRuntime.InvokeVoidAsync("sessionManager.remove", SessionStorageKey);
        }
        catch (Exception ex) when (JsInteropGuard.IsUnavailable(ex))
        {
            // Ignore when JS interop isn't available (for example, during static pre-render).
        }
    }
}

using Microsoft.AspNetCore.Components;
using Shared.Services.Auth;
using Shared.Services.SaasServices;

namespace SMERPWeb.Components.Pages.CommonForms;

public partial class Login : ComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IAuthApiClient AuthApiClient { get; set; } = default!;

    [Inject]
    private ITenantApiClient TenantApiClient { get; set; } = default!;

    [Inject]
    private IUserSessionService UserSessionService { get; set; } = default!;

    protected string TenantEmail { get; set; } = string.Empty;

    protected string UserName { get; set; } = string.Empty;

    protected string Password { get; set; } = string.Empty;

    protected string? ErrorMessage { get; set; }

    protected bool IsLoggingIn { get; set; }

    protected async Task OnLoginClicked()
    {
        if (IsLoggingIn)
        {
            return;
        }

        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(TenantEmail) || string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Tenant email, username, and password are required.";
            return;
        }

        IsLoggingIn = true;

        try
        {
            var response = await AuthApiClient.LoginAsync(new LoginRequest(TenantEmail.Trim(), UserName.Trim(), Password));

            if (response is null)
            {
                ErrorMessage = "Invalid tenant email, username, or password.";
                return;
            }

            var tenantName = $"Tenant #{response.TenantId}";
            var tenant = await TenantApiClient.GetByIdAsync(response.TenantId);
            if (!string.IsNullOrWhiteSpace(tenant?.Name))
            {
                tenantName = tenant.Name;
            }

            await UserSessionService.SetSessionAsync(new UserSession(
                response.Id,
                response.TenantId,
                tenantName,
                response.Username,
                response.DisplayName,
                response.Email,
                response.Mobile,
                response.LastLoginAt));

            NavigationManager.NavigateTo("/module-landing", true);
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Unable to reach the server. Please try again.";
        }
        catch (TaskCanceledException)
        {
            ErrorMessage = "Login request timed out. Please try again.";
        }
        finally
        {
            IsLoggingIn = false;
        }
    }
}

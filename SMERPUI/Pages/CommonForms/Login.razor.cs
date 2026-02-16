using System.ComponentModel.DataAnnotations;
using Domain.SaasDBModels;
using Microsoft.AspNetCore.Components;
using SMERPUI.Services.Auth;
using SMERPUI.Services.SaasServices;

namespace SMERPUI.Pages.CommonForms;

public partial class Login : ComponentBase
{
    [Inject] private ITenantApiClient TenantApiClient { get; set; } = default!;
    [Inject] private IAuthApiClient AuthApiClient { get; set; } = default!;
    [Inject] private IUserSessionService UserSessionService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    protected LoginFormModel LoginModel { get; } = new();
    protected List<Tenant> Tenants { get; private set; } = [];
    protected bool IsSubmitting { get; private set; }
    protected string? ErrorMessage { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        if (await UserSessionService.IsLoggedInAsync())
        {
            NavigationManager.NavigateTo("/dashboard", forceLoad: false);
            return;
        }

        await LoadTenantsAsync();
    }

    protected async Task HandleLoginAsync()
    {
        if (LoginModel.TenantId is null)
        {
            ErrorMessage = "Please select a tenant.";
            return;
        }

        IsSubmitting = true;
        ErrorMessage = null;

        try
        {
            var response = await AuthApiClient.LoginAsync(new LoginRequest(
                LoginModel.TenantId.Value,
                LoginModel.Username.Trim(),
                LoginModel.Password));

            if (response is null)
            {
                ErrorMessage = "Invalid username or password.";
                return;
            }

            await UserSessionService.SetSessionAsync(new UserSession(
                response.Id,
                response.TenantId,
                response.Username,
                response.DisplayName,
                response.Email,
                response.Mobile,
                response.LastLoginAt));

            NavigationManager.NavigateTo("/dashboard", forceLoad: true);
        }
        finally
        {
            IsSubmitting = false;
        }
    }

    private async Task LoadTenantsAsync()
    {
        try
        {
            Tenants = await TenantApiClient.GetAllAsync();
        }
        catch
        {
            ErrorMessage = "Unable to load tenants from API. Please try again.";
        }
    }

    protected sealed class LoginFormModel
    {
        [Required(ErrorMessage = "Tenant is required.")]
        public int? TenantId { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }
}

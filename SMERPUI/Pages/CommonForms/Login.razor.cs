using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Radzen;
using SMERPUI.Services.Auth;
using SMERPUI.Services.SaasServices;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

namespace SMERPUI.Pages.CommonForms;

public partial class Login : ComponentBase
{
    [Inject] private IAuthApiClient AuthApiClient { get; set; } = default!;
    [Inject] private IUserSessionService UserSessionService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    protected LoginFormModel LoginModel { get; } = new();
    protected EditContext LoginEditContext { get; private set; } = default!;
    protected bool IsSubmitting { get; private set; }
    protected string? ErrorMessage { get; private set; }

    protected override void OnInitialized()
    {
        LoginEditContext = new EditContext(LoginModel);
    }

    protected override async Task OnInitializedAsync()
    {
        if (await UserSessionService.IsLoggedInAsync())
        {
            NavigationManager.NavigateTo("/dashboard", forceLoad: false);
        }
    }

    void OnInvalidSubmit(FormInvalidSubmitEventArgs args)
    {
        //Console.WriteLine($"InvalidSubmit: {Newtonsoft.Json.JsonSerializer.Serialize(args, new JsonSerializerOptions() { WriteIndented = true })}");
    }

    // Changed to parameterless handler and use LoginEditContext directly to avoid binding ambiguity.
    protected async Task HandleLoginAsync()
    {
        // LoginEditContext is already set on initialization; use it to validate
        //if (!LoginEditContext.Validate())
        //{
        //    return;
        //}

        IsSubmitting = true;
        ErrorMessage = null;

        try
        {
            var response = await AuthApiClient.LoginAsync(
                new LoginRequest(
                    LoginModel.TenantEmail.Trim(),
                    LoginModel.Username.Trim(),
                    LoginModel.Password),
                CancellationToken.None);

            if (response is null)
            {
                ErrorMessage = "Invalid tenant email, username, or password.";
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

    protected sealed class LoginFormModel
    {
        [Required(ErrorMessage = "Tenant email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid tenant email.")]
        public string TenantEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }
}

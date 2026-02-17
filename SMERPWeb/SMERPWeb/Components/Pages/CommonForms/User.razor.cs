using Domain.SaasDBModels;
using Microsoft.AspNetCore.Components;
using SMERPWeb.Services.SaasServices;

namespace SMERPWeb.Components.Pages.CommonForms;

public partial class User : ComponentBase
{
    [Inject] private IUserOnboardingService UserOnboardingService { get; set; } = default!;

    protected CreateUserWithPermissionRequest FormModel { get; set; } = new();
    protected List<Domain.SaasDBModels.Tenant> Tenants { get; set; } = [];
    protected List<Permission> Permissions { get; set; } = [];
    protected bool IsLoading { get; set; } = true;
    protected string? ErrorMessage { get; set; }
    protected string? SuccessMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Tenants = await UserOnboardingService.GetTenantsAsync();
            Permissions = await UserOnboardingService.GetPermissionsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load dropdown data. {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected async Task CreateUserAsync(CreateUserWithPermissionRequest _)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        try
        {
            var userId = await UserOnboardingService.CreateUserWithPermissionAsync(FormModel);
            SuccessMessage = $"User created successfully with ID {userId}.";
            FormModel = new CreateUserWithPermissionRequest();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}

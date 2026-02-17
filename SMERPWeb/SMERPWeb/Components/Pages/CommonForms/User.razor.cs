using Domain.SaasDBModels;
using Domain.SaasReqDTO;
using Microsoft.AspNetCore.Components;
using SMERPWeb.Services.SaasServices;

namespace SMERPWeb.Components.Pages.CommonForms;

public partial class User : ComponentBase
{
    [Inject] private IUserOnboardingService UserOnboardingService { get; set; } = default!;

    protected CreateUserWithRoleRequest FormModel { get; set; } = new();
    protected List<Tenant> Tenants { get; set; } = [];
    protected List<Role> Roles { get; set; } = [];
    protected bool IsLoading { get; set; } = true;
    protected string? ErrorMessage { get; set; }
    protected string? SuccessMessage { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Tenants = await UserOnboardingService.GetTenantsAsync();
            Roles = await UserOnboardingService.GetRolesAsync();
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

    protected async Task CreateUserAsync(CreateUserWithRoleRequest _)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        try
        {
            var userId = await UserOnboardingService.CreateUserWithRoleAsync(FormModel);
            SuccessMessage = $"User created successfully with ID {userId}.";
            FormModel = new CreateUserWithRoleRequest();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }
}

using Microsoft.AspNetCore.Components;

namespace SMERPWeb.Components.Pages.CommonForms;

public partial class Login : ComponentBase
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    protected string TenantEmail { get; set; } = string.Empty;

    protected string UserName { get; set; } = string.Empty;

    protected string Password { get; set; } = string.Empty;

    protected void OnLoginClicked()
    {
        NavigationManager.NavigateTo("/Home");
    }
}

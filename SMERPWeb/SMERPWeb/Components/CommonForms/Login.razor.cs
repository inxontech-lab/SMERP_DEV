using Microsoft.AspNetCore.Components;

namespace SMERPWeb.Components.CommonForms;

public partial class Login : ComponentBase
{
    protected string TenantEmail { get; set; } = string.Empty;

    protected string UserName { get; set; } = string.Empty;

    protected string Password { get; set; } = string.Empty;
}

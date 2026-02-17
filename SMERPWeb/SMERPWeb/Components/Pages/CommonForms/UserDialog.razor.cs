using Microsoft.AspNetCore.Components;
using Radzen;
using SMERPWeb.Models.User;
using SMERPWeb.Services.SaasServices;

namespace SMERPWeb.Components.Pages.CommonForms;

public partial class UserDialog : ComponentBase
{
    [Inject] private IUserManagementService UserManagementService { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    [Parameter] public List<Domain.SaasDBModels.Tenant> Tenants { get; set; } = [];
    [Parameter] public List<Domain.SaasDBModels.Role> Roles { get; set; } = [];
    [Parameter] public UserFormModel FormModel { get; set; } = new();
    [Parameter] public long? EditingUserId { get; set; }

    protected string? ErrorMessage { get; set; }
    protected IEnumerable<Domain.SaasDBModels.Role> FilteredRoles => Roles.Where(item => item.TenantId == FormModel.TenantId);

    protected override void OnParametersSet()
    {
        if (FormModel.TenantId == 0 && Tenants.Count > 0)
        {
            FormModel.TenantId = Tenants[0].Id;
        }

        OnTenantChanged(FormModel.TenantId);
    }

    protected async Task SaveAsync(UserFormModel _)
    {
        ErrorMessage = null;

        if (!EditingUserId.HasValue && string.IsNullOrWhiteSpace(FormModel.Password))
        {
            ErrorMessage = "Password is required for creating user.";
            NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
            return;
        }

        var action = EditingUserId.HasValue ? "update" : "create";
        var confirmed = await DialogService.Confirm(
            $"Are you sure you want to {action} this user?",
            "Confirm",
            new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed != true)
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Cancelled", "User save operation cancelled.");
            return;
        }

        try
        {
            if (EditingUserId.HasValue)
            {
                var updated = await UserManagementService.UpdateUserAsync(EditingUserId.Value, FormModel.ToUpdateRequest());
                if (!updated)
                {
                    ErrorMessage = "Unable to update user.";
                    NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
                    return;
                }
            }
            else
            {
                await UserManagementService.CreateUserAsync(FormModel.ToCreateRequest());
            }

            NotificationService.Notify(NotificationSeverity.Success, "Success", $"User {(EditingUserId.HasValue ? "updated" : "created")} successfully.");
            DialogService.Close(true);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
        }
    }

    protected void OnTenantChanged(object value)
    {
        var tenantId = value switch
        {
            int intValue => intValue,
            _ => FormModel.TenantId
        };

        FormModel.TenantId = tenantId;

        if (!FilteredRoles.Any(item => item.Id == FormModel.RoleId))
        {
            FormModel.RoleId = FilteredRoles.FirstOrDefault()?.Id ?? 0;
        }
    }

    protected void Cancel() => DialogService.Close(false);
}

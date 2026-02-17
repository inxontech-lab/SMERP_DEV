using Domain.SaasDBModels;
using Domain.SaasReqDTO;
using Microsoft.AspNetCore.Components;
using Radzen;
using SMERPWeb.Models.User;
using SMERPWeb.Services.SaasServices;

namespace SMERPWeb.Components.Pages.CommonForms;

public partial class User : ComponentBase
{
    [Inject] private IUserManagementService UserManagementService { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    protected List<Domain.SaasDBModels.Tenant> Tenants { get; set; } = [];
    protected List<Domain.SaasDBModels.Role> Roles { get; set; } = [];
    protected List<UserWithRoleResponse> Users { get; set; } = [];
    protected UserFormModel FormModel { get; set; } = new();
    protected long? EditingId { get; set; }
    protected string? ErrorMessage { get; set; }
    protected string? SuccessMessage { get; set; }

    protected IEnumerable<Domain.SaasDBModels.Role> FilteredRoles => Roles.Where(item => item.TenantId == FormModel.TenantId);

    protected override async Task OnInitializedAsync() => await LoadAsync();

    protected async Task SaveAsync(UserFormModel _)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        if (!EditingId.HasValue && string.IsNullOrWhiteSpace(FormModel.Password))
        {
            ErrorMessage = "Password is required for creating user.";
            NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
            return;
        }

        var action = EditingId.HasValue ? "update" : "create";
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
            if (EditingId.HasValue)
            {
                var updated = await UserManagementService.UpdateUserAsync(EditingId.Value, FormModel.ToUpdateRequest());
                if (!updated)
                {
                    ErrorMessage = "Unable to update user.";
                    NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
                    return;
                }

                SuccessMessage = "User updated successfully.";
            }
            else
            {
                await UserManagementService.CreateUserAsync(FormModel.ToCreateRequest());
                SuccessMessage = "User created successfully.";
            }

            NotificationService.Notify(NotificationSeverity.Success, "Success", SuccessMessage);
            await LoadUsersAsync();
            ResetForm();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
        }
    }

    protected void Edit(UserWithRoleResponse item)
    {
        EditingId = item.UserId;
        FormModel = UserFormModel.FromResponse(item);
    }

    protected async Task DeleteAsync(long userId)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        var confirmed = await DialogService.Confirm(
            "Are you sure you want to delete this user?",
            "Confirm",
            new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed != true)
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Cancelled", "User delete operation cancelled.");
            return;
        }

        try
        {
            var deleted = await UserManagementService.DeleteUserAsync(userId);
            if (!deleted)
            {
                ErrorMessage = "Unable to delete user.";
                NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
                return;
            }

            if (EditingId == userId)
            {
                ResetForm();
            }

            SuccessMessage = "User deleted successfully.";
            NotificationService.Notify(NotificationSeverity.Success, "Success", SuccessMessage);
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
        }
    }

    protected void ResetForm()
    {
        EditingId = null;
        ErrorMessage = null;
        SuccessMessage = null;

        FormModel = new UserFormModel
        {
            IsActive = true,
            TenantId = Tenants.FirstOrDefault()?.Id ?? 0
        };

        OnTenantChanged(FormModel.TenantId);
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

    private async Task LoadAsync()
    {
        Tenants = await UserManagementService.GetTenantsAsync();
        Roles = await UserManagementService.GetRolesAsync();
        await LoadUsersAsync();
        ResetForm();
    }

    private async Task LoadUsersAsync() => Users = await UserManagementService.GetUsersAsync();
}

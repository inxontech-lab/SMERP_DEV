using Domain.SaasReqDTO;
using Microsoft.AspNetCore.Components;
using Radzen;
using SMERPWeb.Models.User;
using SMERPWeb.Services.Auth;
using SMERPWeb.Services.SaasServices;

namespace SMERPWeb.Components.Pages.CommonForms;

public partial class User : ComponentBase
{
    [Inject] private IUserManagementService UserManagementService { get; set; } = default!;
    [Inject] private ICrudPermissionService CrudPermissionService { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    protected List<Domain.SaasDBModels.Tenant> Tenants { get; set; } = [];
    protected List<Domain.SaasDBModels.Role> Roles { get; set; } = [];
    protected List<UserWithRoleResponse> Users { get; set; } = [];
    protected string? ErrorMessage { get; set; }
    protected bool CanCreateUser { get; set; }
    protected bool CanEditUser { get; set; }
    protected bool CanDeleteUser { get; set; }

    protected override async Task OnInitializedAsync() => await LoadAsync();

    protected async Task OpenCreateDialogAsync()
    {
        if (!CanCreateUser)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to create users.");
            return;
        }

        await OpenUserDialogAsync("Create User", null, null);
    }

    protected async Task OpenEditDialogAsync(UserWithRoleResponse user)
    {
        if (!CanEditUser)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to edit users.");
            return;
        }

        await OpenUserDialogAsync("Edit User", user.UserId, UserFormModel.FromResponse(user));
    }

    protected async Task DeleteAsync(long userId)
    {
        if (!CanDeleteUser)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Access denied", "You do not have permission to delete users.");
            return;
        }

        ErrorMessage = null;

        var confirmed = await DialogService.Confirm(
            "Are you sure you want to delete this user?",
            "Confirm",
            new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed != true)
        {
            NotifyTopRight(NotificationSeverity.Warning, "Cancelled", "User delete operation cancelled.");
            return;
        }

        try
        {
            var deleted = await UserManagementService.DeleteUserAsync(userId);
            if (!deleted)
            {
                ErrorMessage = "Unable to delete user.";
                NotifyTopRight(NotificationSeverity.Error, "Failed", ErrorMessage);
                return;
            }

            NotifyTopRight(NotificationSeverity.Success, "Success", "User deleted successfully.");
            await LoadUsersAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            NotifyTopRight(NotificationSeverity.Error, "Failed", ErrorMessage);
        }
    }

    protected string GetTenantName(int tenantId)
        => Tenants.FirstOrDefault(t => t.Id == tenantId)?.Name ?? "-";

    private async Task OpenUserDialogAsync(string title, long? editingUserId, UserFormModel? model)
    {
        var result = await DialogService.OpenAsync<UserDialog>(title, new Dictionary<string, object>
        {
            [nameof(UserDialog.Tenants)] = Tenants,
            [nameof(UserDialog.Roles)] = Roles,
            [nameof(UserDialog.EditingUserId)] = editingUserId,
            [nameof(UserDialog.FormModel)] = model ?? new UserFormModel
            {
                IsActive = true,
                TenantId = Tenants.FirstOrDefault()?.Id ?? 0
            }
        }, new DialogOptions
        {
            Width = "720px",
            Resizable = true,
            Draggable = true,
            CloseDialogOnEsc = true
        });

        if (result is true)
        {
            await LoadUsersAsync();
            NotifyTopRight(NotificationSeverity.Success, "Success", editingUserId.HasValue ? "User updated successfully." : "User created successfully.");
        }
    }


    private void NotifyTopRight(NotificationSeverity severity, string summary, string? detail)
    {
        NotificationService.Notify(new NotificationMessage
        {
            Severity = severity,
            Summary = summary,
            Detail = detail,
            Duration = 4000,
            CloseOnClick = true,
        });
    }

    private async Task LoadAsync()
    {
        await LoadAccessAsync();
        Tenants = await UserManagementService.GetTenantsAsync();
        Roles = await UserManagementService.GetRolesAsync();
        await LoadUsersAsync();
    }

    private async Task LoadAccessAsync()
    {
        var permissions = await CrudPermissionService.GetPermissionsAsync("user");
        CanCreateUser = permissions.CanCreate;
        CanEditUser = permissions.CanEdit;
        CanDeleteUser = permissions.CanDelete;
    }

    private async Task LoadUsersAsync() => Users = await UserManagementService.GetUsersAsync();
}

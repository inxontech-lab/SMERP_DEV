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
    protected string? ErrorMessage { get; set; }
    protected string? SuccessMessage { get; set; }

    protected override async Task OnInitializedAsync() => await LoadAsync();

    protected async Task OpenCreateDialogAsync()
    {
        await OpenUserDialogAsync("Create User", null, null);
    }

    protected async Task OpenEditDialogAsync(UserWithRoleResponse user)
    {
        await OpenUserDialogAsync("Edit User", user.UserId, UserFormModel.FromResponse(user));
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
            SuccessMessage = editingUserId.HasValue ? "User updated successfully." : "User created successfully.";
        }
    }

    private async Task LoadAsync()
    {
        Tenants = await UserManagementService.GetTenantsAsync();
        Roles = await UserManagementService.GetRolesAsync();
        await LoadUsersAsync();
    }

    private async Task LoadUsersAsync() => Users = await UserManagementService.GetUsersAsync();
}

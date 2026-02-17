using Domain.SaasDBModels;
using Domain.SaasReqDTO;
using Microsoft.AspNetCore.Components;
using Radzen;
using SMERPWeb.Services.SaasServices;

namespace SMERPWeb.Components.Pages.CommonForms;

public partial class RolePermission : ComponentBase
{
    [Inject] private ITenantManagementApiClient TenantApiClient { get; set; } = default!;
    [Inject] private IRoleApiClient RoleApiClient { get; set; } = default!;
    [Inject] private IPermissionApiClient PermissionApiClient { get; set; } = default!;
    [Inject] private IRolePermissionManagementApiClient RolePermissionApiClient { get; set; } = default!;
    [Inject] private DialogService DialogService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    protected List<Domain.SaasDBModels.Tenant> Tenants { get; set; } = [];
    protected List<Domain.SaasDBModels.Role> Roles { get; set; } = [];
    protected List<Permission> Permissions { get; set; } = [];
    protected List<Domain.SaasDBModels.RolePermission> RolePermissions { get; set; } = [];

    protected RolePermissionRequest FormModel { get; set; } = new();
    protected (int TenantId, int RoleId, int PermissionId)? EditingKeys { get; set; }
    protected bool IsEditing => EditingKeys.HasValue;

    protected string? ErrorMessage { get; set; }
    protected string? SuccessMessage { get; set; }

    protected override async Task OnInitializedAsync() => await LoadAsync();

    protected async Task SaveAsync(RolePermissionRequest _)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        var action = IsEditing ? "update" : "create";
        var confirmed = await DialogService.Confirm(
            $"Are you sure you want to {action} this role permission mapping?",
            "Confirm",
            new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed != true)
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Cancelled", "Operation cancelled.");
            return;
        }

        try
        {
            if (EditingKeys.HasValue)
            {
                var (tenantId, roleId, permissionId) = EditingKeys.Value;
                var updated = await RolePermissionApiClient.UpdateAsync(tenantId, roleId, permissionId, FormModel);
                if (!updated)
                {
                    ErrorMessage = "Unable to update role permission mapping.";
                    NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
                    return;
                }

                SuccessMessage = "Role permission mapping updated successfully.";
            }
            else
            {
                await RolePermissionApiClient.CreateAsync(FormModel);
                SuccessMessage = "Role permission mapping created successfully.";
            }

            NotificationService.Notify(NotificationSeverity.Success, "Success", SuccessMessage);
            await LoadRolePermissionsAsync();
            ResetForm();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
        }
    }

    protected void Edit(Domain.SaasDBModels.RolePermission item)
    {
        EditingKeys = (item.TenantId, item.RoleId, item.PermissionId);
        FormModel = new RolePermissionRequest
        {
            TenantId = item.TenantId,
            RoleId = item.RoleId,
            PermissionId = item.PermissionId
        };
    }

    protected async Task DeleteAsync(int tenantId, int roleId, int permissionId)
    {
        ErrorMessage = null;
        SuccessMessage = null;

        var confirmed = await DialogService.Confirm(
            "Are you sure you want to delete this role permission mapping?",
            "Confirm",
            new ConfirmOptions { OkButtonText = "Yes", CancelButtonText = "No" });

        if (confirmed != true)
        {
            NotificationService.Notify(NotificationSeverity.Warning, "Cancelled", "Delete operation cancelled.");
            return;
        }

        try
        {
            var deleted = await RolePermissionApiClient.DeleteAsync(tenantId, roleId, permissionId);
            if (!deleted)
            {
                ErrorMessage = "Unable to delete role permission mapping.";
                NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
                return;
            }

            if (EditingKeys == (tenantId, roleId, permissionId))
            {
                ResetForm();
            }

            SuccessMessage = "Role permission mapping deleted successfully.";
            NotificationService.Notify(NotificationSeverity.Success, "Success", SuccessMessage);
            await LoadRolePermissionsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
        }
    }

    protected string GetRoleName(int roleId)
        => Roles.FirstOrDefault(role => role.Id == roleId)?.Name ?? roleId.ToString();

    protected string GetPermissionName(int permissionId)
        => Permissions.FirstOrDefault(permission => permission.Id == permissionId)?.Name ?? permissionId.ToString();

    protected void ResetForm()
    {
        EditingKeys = null;
        ErrorMessage = null;
        SuccessMessage = null;
        FormModel = new RolePermissionRequest();
    }

    private async Task LoadAsync()
    {
        Tenants = await TenantApiClient.GetAllAsync();
        Roles = await RoleApiClient.GetAllAsync();
        Permissions = await PermissionApiClient.GetAllAsync();
        await LoadRolePermissionsAsync();
        ResetForm();
    }

    private async Task LoadRolePermissionsAsync()
        => RolePermissions = await RolePermissionApiClient.GetAllAsync();
}

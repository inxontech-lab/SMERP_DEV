using Domain.SaasReqDTO;
using Microsoft.AspNetCore.Components;
using Radzen;
using Shared.Services.SaasServices;

namespace SMERPWeb.Components.Pages.CommonForms;

public partial class RolePermission : ComponentBase
{
    [Inject] private ITenantManagementApiClient TenantApiClient { get; set; } = default!;
    [Inject] private IRoleManagementApiClient RoleApiClient { get; set; } = default!;
    [Inject] private IPermissionApiClient PermissionApiClient { get; set; } = default!;
    [Inject] private IRolePermissionManagementApiClient RolePermissionApiClient { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    protected List<Domain.SaasDBModels.Tenant> Tenants { get; set; } = [];
    protected List<Domain.SaasDBModels.Role> Roles { get; set; } = [];
    protected List<Domain.SaasDBModels.Role> FilteredRoles { get; set; } = [];
    protected List<Domain.SaasDBModels.Permission> Permissions { get; set; } = [];
    protected HashSet<int> AssignedPermissionIds { get; set; } = [];
    protected HashSet<int> UpdatingPermissionIds { get; } = [];

    protected int? SelectedTenantId { get; set; }
    protected int? SelectedRoleId { get; set; }
    protected bool IsBusy { get; set; }
    protected string? ErrorMessage { get; set; }
    protected string? SuccessMessage { get; set; }

    protected override async Task OnInitializedAsync() => await LoadInitialDataAsync();

    protected async Task OnTenantChanged(object _)
    {
        SelectedRoleId = null;
        AssignedPermissionIds = [];

        if (SelectedTenantId is null)
        {
            FilteredRoles = [];
            return;
        }

        FilteredRoles = Roles.Where(r => r.TenantId == SelectedTenantId.Value && r.IsActive)
                            .OrderBy(r => r.Name)
                            .ToList();

        if (FilteredRoles.Count > 0)
        {
            SelectedRoleId = FilteredRoles[0].Id;
            await LoadRolePermissionsAsync();
        }
    }

    protected async Task OnRoleChanged(object _)
    {
        AssignedPermissionIds = [];
        await LoadRolePermissionsAsync();
    }

    protected async Task TogglePermissionAsync(int permissionId, bool shouldAssign)
    {
        if (SelectedTenantId is null || SelectedRoleId is null)
        {
            return;
        }

        if (!UpdatingPermissionIds.Add(permissionId))
        {
            return;
        }

        ErrorMessage = null;
        SuccessMessage = null;

        try
        {
            if (shouldAssign)
            {
                await RolePermissionApiClient.CreateAsync(new RolePermissionRequest
                {
                    TenantId = SelectedTenantId.Value,
                    RoleId = SelectedRoleId.Value,
                    PermissionId = permissionId
                });

                AssignedPermissionIds.Add(permissionId);
                SuccessMessage = "Permission assigned successfully.";
            }
            else
            {
                var deleted = await RolePermissionApiClient.DeleteAsync(SelectedTenantId.Value, SelectedRoleId.Value, permissionId);
                if (!deleted)
                {
                    ErrorMessage = "Unable to remove permission mapping.";
                    NotificationService.Notify(NotificationSeverity.Warning, "Warning", ErrorMessage);
                    return;
                }

                AssignedPermissionIds.Remove(permissionId);
                SuccessMessage = "Permission removed successfully.";
            }

            NotificationService.Notify(NotificationSeverity.Success, "Success", SuccessMessage);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
        }
        finally
        {
            UpdatingPermissionIds.Remove(permissionId);
        }
    }

    protected async Task LoadRolePermissionsAsync()
    {
        if (SelectedTenantId is null || SelectedRoleId is null)
        {
            return;
        }

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            var mappings = await RolePermissionApiClient.GetAllAsync();
            AssignedPermissionIds = mappings
                .Where(rp => rp.TenantId == SelectedTenantId.Value && rp.RoleId == SelectedRoleId.Value)
                .Select(rp => rp.PermissionId)
                .ToHashSet();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadInitialDataAsync()
    {
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            Tenants = await TenantApiClient.GetAllAsync();
            Roles = await RoleApiClient.GetAllAsync();
            Permissions = (await PermissionApiClient.GetAllAsync())
                .OrderBy(p => p.Module)
                .ThenBy(p => p.Name)
                .ToList();

            if (Tenants.Count > 0)
            {
                SelectedTenantId = Tenants[0].Id;
                await OnTenantChanged(SelectedTenantId);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            NotificationService.Notify(NotificationSeverity.Error, "Failed", ErrorMessage);
        }
        finally
        {
            IsBusy = false;
        }
    }
}

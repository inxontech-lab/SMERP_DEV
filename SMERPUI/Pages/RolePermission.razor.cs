using Domain.SaasDBModels;
using Domain.SaasReqDTO;
using Microsoft.AspNetCore.Components;
using Radzen;
using SMERPUI.Services.Auth;
using SMERPUI.Services.SaasServices;

namespace SMERPUI.Pages;

public partial class RolePermission : ComponentBase
{
    [Inject] private IRoleApiClient RoleApiClient { get; set; } = default!;
    [Inject] private IPermissionApiClient PermissionApiClient { get; set; } = default!;
    [Inject] private IRolePermissionApiClient RolePermissionApiClient { get; set; } = default!;
    [Inject] private IUserSessionService UserSessionService { get; set; } = default!;
    [Inject] private NotificationService NotificationService { get; set; } = default!;

    protected List<Role> Roles { get; private set; } = [];
    protected List<Permission> Permissions { get; private set; } = [];
    protected HashSet<int> AssignedPermissionIds { get; private set; } = [];
    protected HashSet<int> IsUpdating { get; } = [];
    protected int? SelectedRoleId { get; set; }
    protected bool IsLoading { get; private set; }
    private int _tenantId;

    protected override async Task OnInitializedAsync()
    {
        IsLoading = true;

        try
        {
            var session = await UserSessionService.GetSessionAsync();
            if (session is null)
            {
                NotificationService.Notify(NotificationSeverity.Warning, "Session missing", "Please login again.");
                return;
            }

            _tenantId = session.TenantId;

            var roleItems = await RoleApiClient.GetAllAsync();
            var permissionItems = await PermissionApiClient.GetAllAsync();

            Roles = roleItems.Where(r => r.TenantId == _tenantId && r.IsActive).OrderBy(r => r.Name).ToList();
            Permissions = permissionItems.OrderBy(p => p.Module).ThenBy(p => p.Name).ToList();

            if (Roles.Count > 0)
            {
                SelectedRoleId = Roles[0].Id;
                await LoadRolePermissionsAsync(SelectedRoleId.Value);
            }
        }
        catch (Exception ex)
        {
            NotificationService.Notify(NotificationSeverity.Error, "Load failed", ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected async Task OnRoleChanged(object value)
    {
        if (value is not int roleId)
        {
            return;
        }

        SelectedRoleId = roleId;
        await LoadRolePermissionsAsync(roleId);
    }

    private async Task LoadRolePermissionsAsync(int roleId)
    {
        IsLoading = true;

        try
        {
            var mappings = await RolePermissionApiClient.GetAllAsync();
            AssignedPermissionIds = mappings
                .Where(rp => rp.TenantId == _tenantId && rp.RoleId == roleId)
                .Select(rp => rp.PermissionId)
                .ToHashSet();
        }
        catch (Exception ex)
        {
            NotificationService.Notify(NotificationSeverity.Error, "Unable to load role permissions", ex.Message);
        }
        finally
        {
            IsLoading = false;
        }
    }

    protected bool IsPermissionAssigned(int permissionId) => AssignedPermissionIds.Contains(permissionId);

    protected async Task OnPermissionToggleAsync(int permissionId, bool isChecked)
    {
        if (SelectedRoleId is null)
        {
            return;
        }

        if (!IsUpdating.Add(permissionId))
        {
            return;
        }

        var roleId = SelectedRoleId.Value;

        try
        {
            if (isChecked)
            {
                await RolePermissionApiClient.CreateAsync(new RolePermissionRequest
                {
                    TenantId = _tenantId,
                    RoleId = roleId,
                    PermissionId = permissionId
                });

                AssignedPermissionIds.Add(permissionId);
                NotificationService.Notify(NotificationSeverity.Success, "Permission assigned");
            }
            else
            {
                var deleted = await RolePermissionApiClient.DeleteAsync(_tenantId, roleId, permissionId);
                if (!deleted)
                {
                    NotificationService.Notify(NotificationSeverity.Warning, "Permission not removed", "The selected mapping may not exist anymore.");
                    return;
                }

                AssignedPermissionIds.Remove(permissionId);
                NotificationService.Notify(NotificationSeverity.Success, "Permission removed");
            }
        }
        catch (Exception ex)
        {
            NotificationService.Notify(NotificationSeverity.Error, "Permission update failed", ex.Message);
        }
        finally
        {
            IsUpdating.Remove(permissionId);
        }
    }
}

using Domain.SaasDBModels;

namespace SMERPAPIs.Services.SaasServices;

public interface IRolePermissionService
{
    Task<List<RolePermission>> GetAllAsync();
    Task<RolePermission?> GetByIdAsync(int tenantId, int roleId, int permissionId);
    Task<RolePermission> CreateAsync(RolePermission entity);
    Task<bool> UpdateAsync(int tenantId, int roleId, int permissionId, RolePermission entity);
    Task<bool> DeleteAsync(int tenantId, int roleId, int permissionId);
}

using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public interface IRolePermissionService
{
    Task<List<RolePermission>> GetAllAsync();
    Task<RolePermission?> GetByIdAsync(int tenantId, int roleId, int permissionId);
    Task<RolePermission> CreateAsync(RolePermissionRequest request);
    Task<bool> UpdateAsync(int tenantId, int roleId, int permissionId, RolePermissionRequest request);
    Task<bool> DeleteAsync(int tenantId, int roleId, int permissionId);
}

using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public interface IUserRoleService
{
    Task<List<UserRole>> GetAllAsync();
    Task<UserRole?> GetByIdAsync(int tenantId, long userId, int roleId);
    Task<UserRole> CreateAsync(UserRoleRequest request);
    Task<bool> UpdateAsync(int tenantId, long userId, int roleId, UserRoleRequest request);
    Task<bool> DeleteAsync(int tenantId, long userId, int roleId);
}

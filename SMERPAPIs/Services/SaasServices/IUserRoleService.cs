using Domain.SaasDBModels;

namespace SMERPAPIs.Services.SaasServices;

public interface IUserRoleService
{
    Task<List<UserRole>> GetAllAsync();
    Task<UserRole?> GetByIdAsync(int tenantId, long userId, int roleId);
    Task<UserRole> CreateAsync(UserRole entity);
    Task<bool> UpdateAsync(int tenantId, long userId, int roleId, UserRole entity);
    Task<bool> DeleteAsync(int tenantId, long userId, int roleId);
}

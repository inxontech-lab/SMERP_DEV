using Domain.SaasDBModels;

namespace SMERPAPIs.Services.SaasServices;

public interface IUserBranchService
{
    Task<List<UserBranch>> GetAllAsync();
    Task<UserBranch?> GetByIdAsync(int tenantId, long userId, int branchId);
    Task<UserBranch> CreateAsync(UserBranch entity);
    Task<bool> UpdateAsync(int tenantId, long userId, int branchId, UserBranch entity);
    Task<bool> DeleteAsync(int tenantId, long userId, int branchId);
}

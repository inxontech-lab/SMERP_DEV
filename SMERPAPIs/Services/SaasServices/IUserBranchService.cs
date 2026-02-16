using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public interface IUserBranchService
{
    Task<List<UserBranch>> GetAllAsync();
    Task<UserBranch?> GetByIdAsync(int tenantId, long userId, int branchId);
    Task<UserBranch> CreateAsync(UserBranchRequest request);
    Task<bool> UpdateAsync(int tenantId, long userId, int branchId, UserBranchRequest request);
    Task<bool> DeleteAsync(int tenantId, long userId, int branchId);
}

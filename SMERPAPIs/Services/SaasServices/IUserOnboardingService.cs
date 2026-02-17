using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.SaasServices;

public interface IUserOnboardingService
{
    Task<List<UserWithRoleResponse>> GetUsersWithRolesAsync();
    Task<long> CreateUserWithRoleAsync(CreateUserWithRoleRequest request);
    Task<bool> UpdateUserWithRoleAsync(long userId, UpdateUserWithRoleRequest request);
    Task<bool> DeleteUserWithRoleAsync(long userId);
}

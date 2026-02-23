using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPUI.Services.SaasServices;

public interface IUserOnboardingService
{
    Task<List<Tenant>> GetTenantsAsync(CancellationToken cancellationToken = default);
    Task<List<Role>> GetRolesAsync(CancellationToken cancellationToken = default);
    Task<List<UserWithRoleResponse>> GetUsersWithRolesAsync(CancellationToken cancellationToken = default);
    Task<long> CreateUserWithRoleAsync(CreateUserWithRoleRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserWithRoleAsync(long userId, UpdateUserWithRoleRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserWithRoleAsync(long userId, CancellationToken cancellationToken = default);
}

public class UserOnboardingService(
    ITenantApiClient tenantApiClient,
    IRoleApiClient roleApiClient,
    IUserOnboardingApiClient userOnboardingApiClient) : IUserOnboardingService
{
    public Task<List<Tenant>> GetTenantsAsync(CancellationToken cancellationToken = default)
        => tenantApiClient.GetAllAsync(cancellationToken);

    public Task<List<Role>> GetRolesAsync(CancellationToken cancellationToken = default)
        => roleApiClient.GetAllAsync(cancellationToken);

    public Task<List<UserWithRoleResponse>> GetUsersWithRolesAsync(CancellationToken cancellationToken = default)
        => userOnboardingApiClient.GetUsersWithRolesAsync(cancellationToken);

    public Task<long> CreateUserWithRoleAsync(CreateUserWithRoleRequest request, CancellationToken cancellationToken = default)
        => userOnboardingApiClient.CreateUserWithRoleAsync(request, cancellationToken);

    public Task<bool> UpdateUserWithRoleAsync(long userId, UpdateUserWithRoleRequest request, CancellationToken cancellationToken = default)
        => userOnboardingApiClient.UpdateUserWithRoleAsync(userId, request, cancellationToken);

    public Task<bool> DeleteUserWithRoleAsync(long userId, CancellationToken cancellationToken = default)
        => userOnboardingApiClient.DeleteUserWithRoleAsync(userId, cancellationToken);
}

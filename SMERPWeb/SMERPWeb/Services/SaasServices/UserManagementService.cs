using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPWeb.Services.SaasServices;

public interface IUserManagementService
{
    Task<List<Tenant>> GetTenantsAsync(CancellationToken cancellationToken = default);
    Task<List<Role>> GetRolesAsync(CancellationToken cancellationToken = default);
    Task<List<UserWithRoleResponse>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<long> CreateUserAsync(CreateUserWithRoleRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserAsync(long userId, UpdateUserWithRoleRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(long userId, CancellationToken cancellationToken = default);
}

public class UserManagementService(
    ITenantApiClient tenantApiClient,
    IRoleApiClient roleApiClient,
    IUserManagementApiClient userManagementApiClient) : IUserManagementService
{
    public Task<List<Tenant>> GetTenantsAsync(CancellationToken cancellationToken = default)
        => tenantApiClient.GetAllAsync(cancellationToken);

    public Task<List<Role>> GetRolesAsync(CancellationToken cancellationToken = default)
        => roleApiClient.GetAllAsync(cancellationToken);

    public Task<List<UserWithRoleResponse>> GetUsersAsync(CancellationToken cancellationToken = default)
        => userManagementApiClient.GetUsersAsync(cancellationToken);

    public Task<long> CreateUserAsync(CreateUserWithRoleRequest request, CancellationToken cancellationToken = default)
        => userManagementApiClient.CreateUserAsync(request, cancellationToken);

    public Task<bool> UpdateUserAsync(long userId, UpdateUserWithRoleRequest request, CancellationToken cancellationToken = default)
        => userManagementApiClient.UpdateUserAsync(userId, request, cancellationToken);

    public Task<bool> DeleteUserAsync(long userId, CancellationToken cancellationToken = default)
        => userManagementApiClient.DeleteUserAsync(userId, cancellationToken);
}

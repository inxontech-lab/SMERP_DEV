using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace Shared.Services.SaasServices;

public interface IUserManagementService
{
    Task<List<Tenant>> GetTenantsAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<List<Role>> GetRolesAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<List<UserWithRoleResponse>> GetUsersAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<long> CreateUserAsync(CreateUserWithRoleRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserAsync(long userId, UpdateUserWithRoleRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(long userId, CancellationToken cancellationToken = default);
}

public class UserManagementService(
    ITenantApiClient tenantApiClient,
    IRoleApiClient roleApiClient,
    IUserManagementApiClient userManagementApiClient) : IUserManagementService
{
    public async Task<List<Tenant>> GetTenantsAsync(int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var tenants = await tenantApiClient.GetAllAsync(cancellationToken);
        return viewerTenantId == 1 ? tenants : tenants.Where(item => item.Id == viewerTenantId).ToList();
    }

    public async Task<List<Role>> GetRolesAsync(int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var roles = await roleApiClient.GetAllAsync(cancellationToken);
        return viewerTenantId == 1 ? roles : roles.Where(item => item.TenantId == viewerTenantId).ToList();
    }

    public Task<List<UserWithRoleResponse>> GetUsersAsync(int viewerTenantId, CancellationToken cancellationToken = default)
        => userManagementApiClient.GetUsersAsync(viewerTenantId, cancellationToken);

    public Task<long> CreateUserAsync(CreateUserWithRoleRequest request, CancellationToken cancellationToken = default)
        => userManagementApiClient.CreateUserAsync(request, cancellationToken);

    public Task<bool> UpdateUserAsync(long userId, UpdateUserWithRoleRequest request, CancellationToken cancellationToken = default)
        => userManagementApiClient.UpdateUserAsync(userId, request, cancellationToken);

    public Task<bool> DeleteUserAsync(long userId, CancellationToken cancellationToken = default)
        => userManagementApiClient.DeleteUserAsync(userId, cancellationToken);
}

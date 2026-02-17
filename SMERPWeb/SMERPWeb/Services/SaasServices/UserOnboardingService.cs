using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPWeb.Services.SaasServices;

public interface IUserOnboardingService
{
    Task<List<Tenant>> GetTenantsAsync(CancellationToken cancellationToken = default);
    Task<List<Role>> GetRolesAsync(CancellationToken cancellationToken = default);
    Task<long> CreateUserWithRoleAsync(CreateUserWithRoleRequest request, CancellationToken cancellationToken = default);
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

    public Task<long> CreateUserWithRoleAsync(CreateUserWithRoleRequest request, CancellationToken cancellationToken = default)
        => userOnboardingApiClient.CreateUserWithRoleAsync(request, cancellationToken);
}

using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Shared.Services.Auth;
using Shared.Services.SaasServices;

namespace Shared.Services.InventoryServices;

public interface IUomManagementService
{
    Task<int> GetViewerTenantIdAsync(CancellationToken cancellationToken = default);
    Task<List<InvUom>> GetUomsAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<List<Tenant>> GetTenantsAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvUom> CreateAsync(CreateInvUomRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateInvUomRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
}

public class UomManagementService(
    IUserSessionService userSessionService,
    IInvUomApiClient uomApiClient,
    ITenantManagementApiClient tenantManagementApiClient) : IUomManagementService
{
    public async Task<int> GetViewerTenantIdAsync(CancellationToken cancellationToken = default)
    {
        var session = await userSessionService.GetSessionAsync();
        return session?.TenantId ?? 0;
    }

    public Task<List<InvUom>> GetUomsAsync(int viewerTenantId, CancellationToken cancellationToken = default)
        => uomApiClient.GetUomsAsync(viewerTenantId, cancellationToken);

    public Task<InvUom> CreateAsync(CreateInvUomRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
        => uomApiClient.CreateUomAsync(request, viewerTenantId, cancellationToken);

    public Task<bool> UpdateAsync(int id, UpdateInvUomRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
        => uomApiClient.UpdateUomAsync(id, request, viewerTenantId, cancellationToken);

    public Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
        => uomApiClient.DeleteUomAsync(id, viewerTenantId, cancellationToken);

    public async Task<List<Tenant>> GetTenantsAsync(int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var tenants = await tenantManagementApiClient.GetAllAsync(cancellationToken);
        return viewerTenantId <= 1 ? tenants : tenants.Where(tenant => tenant.Id == viewerTenantId).ToList();
    }
}

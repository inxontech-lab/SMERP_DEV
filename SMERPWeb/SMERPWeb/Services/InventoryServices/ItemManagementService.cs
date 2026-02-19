using Domain.SaasDBModels;
using Domain.SaasReqDTO;
using SMERPWeb.Services.Auth;
using SMERPWeb.Services.SaasServices;

namespace SMERPWeb.Services.InventoryServices;

public interface IItemManagementService
{
    Task<int> GetViewerTenantIdAsync(CancellationToken cancellationToken = default);
    Task<List<Product>> GetItemsAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<List<Tenant>> GetTenantsAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<List<Uom>> GetUomsAsync(CancellationToken cancellationToken = default);
    Task<Product> CreateAsync(ProductRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(long id, ProductRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, int viewerTenantId, CancellationToken cancellationToken = default);
}

public class ItemManagementService(
    IUserSessionService userSessionService,
    IItemApiClient itemApiClient,
    ITenantManagementApiClient tenantManagementApiClient,
    IUomApiClient uomApiClient) : IItemManagementService
{
    public async Task<int> GetViewerTenantIdAsync(CancellationToken cancellationToken = default)
    {
        var session = await userSessionService.GetSessionAsync();
        return session?.TenantId ?? 0;
    }

    public Task<List<Product>> GetItemsAsync(int viewerTenantId, CancellationToken cancellationToken = default)
        => itemApiClient.GetItemsAsync(viewerTenantId, cancellationToken);

    public Task<List<Uom>> GetUomsAsync(CancellationToken cancellationToken = default)
        => uomApiClient.GetAllAsync(cancellationToken);

    public Task<Product> CreateAsync(ProductRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
        => itemApiClient.CreateItemAsync(request, viewerTenantId, cancellationToken);

    public Task<bool> UpdateAsync(long id, ProductRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
        => itemApiClient.UpdateItemAsync(id, request, viewerTenantId, cancellationToken);

    public Task<bool> DeleteAsync(long id, int viewerTenantId, CancellationToken cancellationToken = default)
        => itemApiClient.DeleteItemAsync(id, viewerTenantId, cancellationToken);

    public async Task<List<Tenant>> GetTenantsAsync(int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var tenants = await tenantManagementApiClient.GetAllAsync(cancellationToken);
        return viewerTenantId == 1 ? tenants : tenants.Where(t => t.Id == viewerTenantId).ToList();
    }
}

using Domain.InvReqDTO;
using Domain.SaasDBModels;
using SMERPWeb.Services.Auth;
using SMERPWeb.Services.SaasServices;

namespace SMERPWeb.Services.InventoryServices;

public interface ISupplierManagementService
{
    Task<int> GetViewerTenantIdAsync(CancellationToken cancellationToken = default);
    Task<List<InvSupplier>> GetSuppliersAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<List<Tenant>> GetTenantsAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvSupplier> CreateAsync(CreateInvSupplierRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateInvSupplierRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
}

public class SupplierManagementService(
    IUserSessionService userSessionService,
    ISupplierApiClient supplierApiClient,
    ITenantManagementApiClient tenantManagementApiClient) : ISupplierManagementService
{
    public async Task<int> GetViewerTenantIdAsync(CancellationToken cancellationToken = default)
    {
        var session = await userSessionService.GetSessionAsync();
        return session?.TenantId ?? 0;
    }

    public Task<List<InvSupplier>> GetSuppliersAsync(int viewerTenantId, CancellationToken cancellationToken = default)
        => supplierApiClient.GetSuppliersAsync(viewerTenantId, cancellationToken);

    public Task<InvSupplier> CreateAsync(CreateInvSupplierRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
        => supplierApiClient.CreateSupplierAsync(request, viewerTenantId, cancellationToken);

    public Task<bool> UpdateAsync(int id, UpdateInvSupplierRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
        => supplierApiClient.UpdateSupplierAsync(id, request, viewerTenantId, cancellationToken);

    public Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
        => supplierApiClient.DeleteSupplierAsync(id, viewerTenantId, cancellationToken);

    public async Task<List<Tenant>> GetTenantsAsync(int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var tenants = await tenantManagementApiClient.GetAllAsync(cancellationToken);
        return viewerTenantId == 1 ? tenants : tenants.Where(t => t.Id == viewerTenantId).ToList();
    }
}

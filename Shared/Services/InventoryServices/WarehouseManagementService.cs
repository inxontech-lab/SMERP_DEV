using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Shared.Services.Auth;
using Shared.Services.SaasServices;

namespace Shared.Services.InventoryServices;

public interface IWarehouseManagementService
{
    Task<int> GetViewerTenantIdAsync(CancellationToken cancellationToken = default);
    Task<List<InvWarehouse>> GetWarehousesAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<List<Tenant>> GetTenantsAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<List<Branch>> GetBranchesAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvWarehouse> CreateAsync(CreateInvWarehouseRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateInvWarehouseRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
}

public class WarehouseManagementService(
    IUserSessionService userSessionService,
    IWarehouseApiClient warehouseApiClient,
    ITenantManagementApiClient tenantManagementApiClient,
    IBranchManagementApiClient branchManagementApiClient) : IWarehouseManagementService
{
    public async Task<int> GetViewerTenantIdAsync(CancellationToken cancellationToken = default)
    {
        var session = await userSessionService.GetSessionAsync();
        return session?.TenantId ?? 0;
    }

    public Task<List<InvWarehouse>> GetWarehousesAsync(int viewerTenantId, CancellationToken cancellationToken = default)
        => warehouseApiClient.GetWarehousesAsync(viewerTenantId, cancellationToken);

    public Task<InvWarehouse> CreateAsync(CreateInvWarehouseRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
        => warehouseApiClient.CreateWarehouseAsync(request, viewerTenantId, cancellationToken);

    public Task<bool> UpdateAsync(int id, UpdateInvWarehouseRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
        => warehouseApiClient.UpdateWarehouseAsync(id, request, viewerTenantId, cancellationToken);

    public Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
        => warehouseApiClient.DeleteWarehouseAsync(id, viewerTenantId, cancellationToken);

    public async Task<List<Tenant>> GetTenantsAsync(int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var tenants = await tenantManagementApiClient.GetAllAsync(cancellationToken);
        return viewerTenantId <= 1 ? tenants : tenants.Where(t => t.Id == viewerTenantId).ToList();
    }

    public async Task<List<Branch>> GetBranchesAsync(int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var branches = await branchManagementApiClient.GetAllAsync(cancellationToken);
        return viewerTenantId <= 1 ? branches : branches.Where(branch => branch.TenantId == viewerTenantId).ToList();
    }
}

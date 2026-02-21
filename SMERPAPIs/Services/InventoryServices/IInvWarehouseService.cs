using Domain.InvReqDTO;
using Domain.SaasDBModels;

namespace SMERPAPIs.Services.InventoryServices;

public interface IInvWarehouseService
{
    Task<List<InvWarehouse>> GetAllAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvWarehouse?> GetByIdAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvWarehouse> CreateAsync(CreateInvWarehouseRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateInvWarehouseRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
}

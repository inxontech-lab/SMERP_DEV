using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPAPIs.Services.InventoryServices;

public interface IItemService
{
    Task<List<Product>> GetAllAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(long id, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<Product> CreateAsync(ProductRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(long id, ProductRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(long id, int viewerTenantId, CancellationToken cancellationToken = default);
}

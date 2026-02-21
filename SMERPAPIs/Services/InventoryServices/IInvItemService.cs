using Domain.InvReqDTO;
using Domain.SaasDBModels;

namespace SMERPAPIs.Services.InventoryServices;

public interface IInvItemService
{
    Task<List<InvItem>> GetAllAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvItem?> GetByIdAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvItem> CreateAsync(CreateInvItemRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateInvItemRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
}

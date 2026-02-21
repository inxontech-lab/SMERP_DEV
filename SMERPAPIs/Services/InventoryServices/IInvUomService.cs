using Domain.InvReqDTO;
using Domain.SaasDBModels;

namespace SMERPAPIs.Services.InventoryServices;

public interface IInvUomService
{
    Task<List<InvUom>> GetAllAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvUom?> GetByIdAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvUom> CreateAsync(CreateInvUomRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateInvUomRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
}

using Domain.InvReqDTO;
using Domain.SaasDBModels;

namespace SMERPAPIs.Services.InventoryServices;

public interface IInvItemUomconversionService
{
    Task<List<InvItemUomconversion>> GetAllAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvItemUomconversion?> GetByIdAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvItemUomconversion> CreateAsync(CreateInvItemUomconversionRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateInvItemUomconversionRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
}

using Domain.InvReqDTO;
using Domain.SaasDBModels;

namespace SMERPAPIs.Services.InventoryServices;

public interface IInvItemSubCategoryService
{
    Task<List<InvItemSubCategory>> GetAllAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvItemSubCategory?> GetByIdAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvItemSubCategory> CreateAsync(CreateInvItemSubCategoryRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateInvItemSubCategoryRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
}

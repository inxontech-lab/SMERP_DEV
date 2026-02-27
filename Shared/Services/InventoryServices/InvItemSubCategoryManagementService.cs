using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Shared.Services.Auth;
using Shared.Services.SaasServices;

namespace Shared.Services.InventoryServices;

public interface IInvItemSubCategoryManagementService
{
    Task<int> GetViewerTenantIdAsync(CancellationToken cancellationToken = default);
    Task<List<InvItemSubCategory>> GetItemSubCategoriesAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<List<InvItemCategory>> GetItemCategoriesAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<List<Tenant>> GetTenantsAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvItemSubCategory> CreateAsync(CreateInvItemSubCategoryRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateInvItemSubCategoryRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
}

public class InvItemSubCategoryManagementService(
    IUserSessionService userSessionService,
    IInvItemSubCategoryApiClient itemSubCategoryApiClient,
    IInvItemCategoryApiClient itemCategoryApiClient,
    ITenantManagementApiClient tenantManagementApiClient) : IInvItemSubCategoryManagementService
{
    public async Task<int> GetViewerTenantIdAsync(CancellationToken cancellationToken = default)
    {
        var session = await userSessionService.GetSessionAsync();
        return session?.TenantId ?? 0;
    }

    public Task<List<InvItemSubCategory>> GetItemSubCategoriesAsync(int viewerTenantId, CancellationToken cancellationToken = default)
        => itemSubCategoryApiClient.GetItemSubCategoriesAsync(viewerTenantId, cancellationToken);

    public Task<List<InvItemCategory>> GetItemCategoriesAsync(int viewerTenantId, CancellationToken cancellationToken = default)
        => itemCategoryApiClient.GetItemCategoriesAsync(viewerTenantId, cancellationToken);

    public Task<InvItemSubCategory> CreateAsync(CreateInvItemSubCategoryRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
        => itemSubCategoryApiClient.CreateItemSubCategoryAsync(request, viewerTenantId, cancellationToken);

    public Task<bool> UpdateAsync(int id, UpdateInvItemSubCategoryRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
        => itemSubCategoryApiClient.UpdateItemSubCategoryAsync(id, request, viewerTenantId, cancellationToken);

    public Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
        => itemSubCategoryApiClient.DeleteItemSubCategoryAsync(id, viewerTenantId, cancellationToken);

    public async Task<List<Tenant>> GetTenantsAsync(int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var tenants = await tenantManagementApiClient.GetAllAsync(cancellationToken);
        return viewerTenantId <= 1 ? tenants : tenants.Where(tenant => tenant.Id == viewerTenantId).ToList();
    }
}

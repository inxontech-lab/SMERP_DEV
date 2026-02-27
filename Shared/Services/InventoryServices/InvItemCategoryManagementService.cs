using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Shared.Services.Auth;
using Shared.Services.SaasServices;

namespace Shared.Services.InventoryServices;

public interface IInvItemCategoryManagementService
{
    Task<int> GetViewerTenantIdAsync(CancellationToken cancellationToken = default);
    Task<List<InvItemCategory>> GetItemCategoriesAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<List<Tenant>> GetTenantsAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvItemCategory> CreateAsync(CreateInvItemCategoryRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateInvItemCategoryRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
}

public class InvItemCategoryManagementService(
    IUserSessionService userSessionService,
    IInvItemCategoryApiClient itemCategoryApiClient,
    ITenantManagementApiClient tenantManagementApiClient) : IInvItemCategoryManagementService
{
    public async Task<int> GetViewerTenantIdAsync(CancellationToken cancellationToken = default)
    {
        var session = await userSessionService.GetSessionAsync();
        return session?.TenantId ?? 0;
    }

    public Task<List<InvItemCategory>> GetItemCategoriesAsync(int viewerTenantId, CancellationToken cancellationToken = default)
        => itemCategoryApiClient.GetItemCategoriesAsync(viewerTenantId, cancellationToken);

    public Task<InvItemCategory> CreateAsync(CreateInvItemCategoryRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
        => itemCategoryApiClient.CreateItemCategoryAsync(request, viewerTenantId, cancellationToken);

    public Task<bool> UpdateAsync(int id, UpdateInvItemCategoryRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
        => itemCategoryApiClient.UpdateItemCategoryAsync(id, request, viewerTenantId, cancellationToken);

    public Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
        => itemCategoryApiClient.DeleteItemCategoryAsync(id, viewerTenantId, cancellationToken);

    public async Task<List<Tenant>> GetTenantsAsync(int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var tenants = await tenantManagementApiClient.GetAllAsync(cancellationToken);
        return viewerTenantId <= 1 ? tenants : tenants.Where(tenant => tenant.Id == viewerTenantId).ToList();
    }
}

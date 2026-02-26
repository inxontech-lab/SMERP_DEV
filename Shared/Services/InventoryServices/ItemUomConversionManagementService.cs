using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Shared.Services.Auth;
using Shared.Services.SaasServices;

namespace Shared.Services.InventoryServices;

public interface IItemUomConversionManagementService
{
    Task<int> GetViewerTenantIdAsync(CancellationToken cancellationToken = default);
    Task<List<InvItemUomconversion>> GetItemUomConversionsAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<List<Tenant>> GetTenantsAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<List<InvItem>> GetItemsAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<List<InvUom>> GetUomsAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvItemUomconversion> CreateAsync(CreateInvItemUomconversionRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, UpdateInvItemUomconversionRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
}

public class ItemUomConversionManagementService(
    IUserSessionService userSessionService,
    IItemUomConversionApiClient itemUomConversionApiClient,
    IInvItemApiClient invItemApiClient,
    IInvUomApiClient invUomApiClient,
    ITenantManagementApiClient tenantManagementApiClient) : IItemUomConversionManagementService
{
    public async Task<int> GetViewerTenantIdAsync(CancellationToken cancellationToken = default)
    {
        var session = await userSessionService.GetSessionAsync();
        return session?.TenantId ?? 0;
    }

    public Task<List<InvItemUomconversion>> GetItemUomConversionsAsync(int viewerTenantId, CancellationToken cancellationToken = default)
        => itemUomConversionApiClient.GetItemUomConversionsAsync(viewerTenantId, cancellationToken);

    public Task<InvItemUomconversion> CreateAsync(CreateInvItemUomconversionRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
        => itemUomConversionApiClient.CreateItemUomConversionAsync(request, viewerTenantId, cancellationToken);

    public Task<bool> UpdateAsync(int id, UpdateInvItemUomconversionRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
        => itemUomConversionApiClient.UpdateItemUomConversionAsync(id, request, viewerTenantId, cancellationToken);

    public Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
        => itemUomConversionApiClient.DeleteItemUomConversionAsync(id, viewerTenantId, cancellationToken);

    public async Task<List<Tenant>> GetTenantsAsync(int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var tenants = await tenantManagementApiClient.GetAllAsync(cancellationToken);
        return viewerTenantId <= 1 ? tenants : tenants.Where(tenant => tenant.Id == viewerTenantId).ToList();
    }

    public async Task<List<InvItem>> GetItemsAsync(int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var items = await invItemApiClient.GetItemsAsync(viewerTenantId, cancellationToken);
        return viewerTenantId <= 1 ? items : items.Where(item => item.TenantId == viewerTenantId).ToList();
    }

    public async Task<List<InvUom>> GetUomsAsync(int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var uoms = await invUomApiClient.GetUomsAsync(viewerTenantId, cancellationToken);
        return viewerTenantId <= 1 ? uoms : uoms.Where(uom => uom.TenantId == viewerTenantId).ToList();
    }
}

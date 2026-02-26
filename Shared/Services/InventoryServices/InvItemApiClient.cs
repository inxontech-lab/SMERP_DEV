using System.Net.Http.Json;
using Domain.SaasDBModels;

namespace Shared.Services.InventoryServices;

public interface IInvItemApiClient
{
    Task<List<InvItem>> GetItemsAsync(int viewerTenantId, CancellationToken cancellationToken = default);
}

public class InvItemApiClient(HttpClient httpClient) : IInvItemApiClient
{
    public async Task<List<InvItem>> GetItemsAsync(int viewerTenantId, CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<InvItem>>($"api/Inventory/InvItems?viewerTenantId={viewerTenantId}", cancellationToken) ?? [];
}

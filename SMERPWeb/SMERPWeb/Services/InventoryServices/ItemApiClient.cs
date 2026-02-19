using System.Net.Http.Json;
using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPWeb.Services.InventoryServices;

public interface IItemApiClient
{
    Task<List<Product>> GetItemsAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<Product> CreateItemAsync(ProductRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateItemAsync(long id, ProductRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteItemAsync(long id, int viewerTenantId, CancellationToken cancellationToken = default);
}

public class ItemApiClient(HttpClient httpClient) : IItemApiClient
{
    public async Task<List<Product>> GetItemsAsync(int viewerTenantId, CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<Product>>($"api/Items?viewerTenantId={viewerTenantId}", cancellationToken) ?? [];

    public async Task<Product> CreateItemAsync(ProductRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"api/Items?viewerTenantId={viewerTenantId}", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<Product>(cancellationToken)
               ?? throw new InvalidOperationException("No Item returned by API.");
    }

    public async Task<bool> UpdateItemAsync(long id, ProductRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
        => (await httpClient.PutAsJsonAsync($"api/Items/{id}?viewerTenantId={viewerTenantId}", request, cancellationToken)).IsSuccessStatusCode;

    public async Task<bool> DeleteItemAsync(long id, int viewerTenantId, CancellationToken cancellationToken = default)
        => (await httpClient.DeleteAsync($"api/Items/{id}?viewerTenantId={viewerTenantId}", cancellationToken)).IsSuccessStatusCode;
}

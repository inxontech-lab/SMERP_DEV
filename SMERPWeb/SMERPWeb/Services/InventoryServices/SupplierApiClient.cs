using System.Net.Http.Json;
using Domain.InvReqDTO;
using Domain.SaasDBModels;

namespace SMERPWeb.Services.InventoryServices;

public interface ISupplierApiClient
{
    Task<List<InvSupplier>> GetSuppliersAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvSupplier> CreateSupplierAsync(CreateInvSupplierRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateSupplierAsync(int id, UpdateInvSupplierRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteSupplierAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
}

public class SupplierApiClient(HttpClient httpClient) : ISupplierApiClient
{
    public async Task<List<InvSupplier>> GetSuppliersAsync(int viewerTenantId, CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<InvSupplier>>($"api/Inventory/InvSuppliers?viewerTenantId={viewerTenantId}", cancellationToken) ?? [];

    public async Task<InvSupplier> CreateSupplierAsync(CreateInvSupplierRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"api/Inventory/InvSuppliers?viewerTenantId={viewerTenantId}", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<InvSupplier>(cancellationToken)
               ?? throw new InvalidOperationException("No supplier returned by API.");
    }

    public async Task<bool> UpdateSupplierAsync(int id, UpdateInvSupplierRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
        => (await httpClient.PutAsJsonAsync($"api/Inventory/InvSuppliers/{id}?viewerTenantId={viewerTenantId}", request, cancellationToken)).IsSuccessStatusCode;

    public async Task<bool> DeleteSupplierAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
        => (await httpClient.DeleteAsync($"api/Inventory/InvSuppliers/{id}?viewerTenantId={viewerTenantId}", cancellationToken)).IsSuccessStatusCode;
}

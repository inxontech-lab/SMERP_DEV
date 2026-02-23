using System.Net.Http.Json;
using Domain.InvReqDTO;
using Domain.SaasDBModels;

namespace SMERPUI.Services.InventoryServices;

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
        await EnsureSuccessWithDetailsAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<InvSupplier>(cancellationToken)
               ?? throw new InvalidOperationException("No supplier returned by API.");
    }

    public async Task<bool> UpdateSupplierAsync(int id, UpdateInvSupplierRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"api/Inventory/InvSuppliers/{id}?viewerTenantId={viewerTenantId}", request, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }

        await EnsureSuccessWithDetailsAsync(response, cancellationToken);
        return false;
    }

    public async Task<bool> DeleteSupplierAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"api/Inventory/InvSuppliers/{id}?viewerTenantId={viewerTenantId}", cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return false;
        }

        await EnsureSuccessWithDetailsAsync(response, cancellationToken);
        return false;
    }

    private static async Task EnsureSuccessWithDetailsAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var details = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!string.IsNullOrWhiteSpace(details))
        {
            throw new InvalidOperationException($"Supplier API error ({(int)response.StatusCode}): {details}");
        }

        response.EnsureSuccessStatusCode();
    }
}

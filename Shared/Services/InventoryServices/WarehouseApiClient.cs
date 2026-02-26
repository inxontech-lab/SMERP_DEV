using System.Net.Http.Json;
using Domain.InvReqDTO;
using Domain.SaasDBModels;

namespace Shared.Services.InventoryServices;

public interface IWarehouseApiClient
{
    Task<List<InvWarehouse>> GetWarehousesAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvWarehouse> CreateWarehouseAsync(CreateInvWarehouseRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateWarehouseAsync(int id, UpdateInvWarehouseRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteWarehouseAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
}

public class WarehouseApiClient(HttpClient httpClient) : IWarehouseApiClient
{
    public async Task<List<InvWarehouse>> GetWarehousesAsync(int viewerTenantId, CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<InvWarehouse>>($"api/Inventory/InvWarehouses?viewerTenantId={viewerTenantId}", cancellationToken) ?? [];

    public async Task<InvWarehouse> CreateWarehouseAsync(CreateInvWarehouseRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"api/Inventory/InvWarehouses?viewerTenantId={viewerTenantId}", request, cancellationToken);
        await EnsureSuccessWithDetailsAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<InvWarehouse>(cancellationToken)
               ?? throw new InvalidOperationException("No warehouse returned by API.");
    }

    public async Task<bool> UpdateWarehouseAsync(int id, UpdateInvWarehouseRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"api/Inventory/InvWarehouses/{id}?viewerTenantId={viewerTenantId}", request, cancellationToken);
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

    public async Task<bool> DeleteWarehouseAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"api/Inventory/InvWarehouses/{id}?viewerTenantId={viewerTenantId}", cancellationToken);
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
            throw new InvalidOperationException($"Warehouse API error ({(int)response.StatusCode}): {details}");
        }

        response.EnsureSuccessStatusCode();
    }
}

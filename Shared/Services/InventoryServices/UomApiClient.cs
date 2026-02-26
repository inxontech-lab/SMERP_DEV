using System.Net.Http.Json;
using Domain.InvReqDTO;
using Domain.SaasDBModels;

namespace Shared.Services.InventoryServices;

public interface IInvUomApiClient
{
    Task<List<InvUom>> GetUomsAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvUom> CreateUomAsync(CreateInvUomRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateUomAsync(int id, UpdateInvUomRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteUomAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
}

public class InvUomApiClient(HttpClient httpClient) : IInvUomApiClient
{
    public async Task<List<InvUom>> GetUomsAsync(int viewerTenantId, CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<InvUom>>($"api/Inventory/InvUoms?viewerTenantId={viewerTenantId}", cancellationToken) ?? [];

    public async Task<InvUom> CreateUomAsync(CreateInvUomRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"api/Inventory/InvUoms?viewerTenantId={viewerTenantId}", request, cancellationToken);
        await EnsureSuccessWithDetailsAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<InvUom>(cancellationToken)
               ?? throw new InvalidOperationException("No UOM returned by API.");
    }

    public async Task<bool> UpdateUomAsync(int id, UpdateInvUomRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"api/Inventory/InvUoms/{id}?viewerTenantId={viewerTenantId}", request, cancellationToken);
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

    public async Task<bool> DeleteUomAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"api/Inventory/InvUoms/{id}?viewerTenantId={viewerTenantId}", cancellationToken);
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
            throw new InvalidOperationException($"UOM API error ({(int)response.StatusCode}): {details}");
        }

        response.EnsureSuccessStatusCode();
    }
}

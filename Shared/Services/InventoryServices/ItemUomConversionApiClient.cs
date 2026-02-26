using System.Net.Http.Json;
using Domain.InvReqDTO;
using Domain.SaasDBModels;

namespace Shared.Services.InventoryServices;

public interface IItemUomConversionApiClient
{
    Task<List<InvItemUomconversion>> GetItemUomConversionsAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvItemUomconversion> CreateItemUomConversionAsync(CreateInvItemUomconversionRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateItemUomConversionAsync(int id, UpdateInvItemUomconversionRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteItemUomConversionAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
}

public class ItemUomConversionApiClient(HttpClient httpClient) : IItemUomConversionApiClient
{
    public async Task<List<InvItemUomconversion>> GetItemUomConversionsAsync(int viewerTenantId, CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<InvItemUomconversion>>($"api/Inventory/InvItemUomconversions?viewerTenantId={viewerTenantId}", cancellationToken) ?? [];

    public async Task<InvItemUomconversion> CreateItemUomConversionAsync(CreateInvItemUomconversionRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"api/Inventory/InvItemUomconversions?viewerTenantId={viewerTenantId}", request, cancellationToken);
        await EnsureSuccessWithDetailsAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<InvItemUomconversion>(cancellationToken)
               ?? throw new InvalidOperationException("No item UOM conversion returned by API.");
    }

    public async Task<bool> UpdateItemUomConversionAsync(int id, UpdateInvItemUomconversionRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"api/Inventory/InvItemUomconversions/{id}?viewerTenantId={viewerTenantId}", request, cancellationToken);
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

    public async Task<bool> DeleteItemUomConversionAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"api/Inventory/InvItemUomconversions/{id}?viewerTenantId={viewerTenantId}", cancellationToken);
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
            throw new InvalidOperationException($"Item UOM conversion API error ({(int)response.StatusCode}): {details}");
        }

        response.EnsureSuccessStatusCode();
    }
}

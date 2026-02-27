using System.Net.Http.Json;
using Domain.InvReqDTO;
using Domain.SaasDBModels;

namespace Shared.Services.InventoryServices;

public interface IInvItemCategoryApiClient
{
    Task<List<InvItemCategory>> GetItemCategoriesAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvItemCategory> CreateItemCategoryAsync(CreateInvItemCategoryRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateItemCategoryAsync(int id, UpdateInvItemCategoryRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteItemCategoryAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
}

public class InvItemCategoryApiClient(HttpClient httpClient) : IInvItemCategoryApiClient
{
    public async Task<List<InvItemCategory>> GetItemCategoriesAsync(int viewerTenantId, CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<InvItemCategory>>($"api/Inventory/InvItemCategories?viewerTenantId={viewerTenantId}", cancellationToken) ?? [];

    public async Task<InvItemCategory> CreateItemCategoryAsync(CreateInvItemCategoryRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"api/Inventory/InvItemCategories?viewerTenantId={viewerTenantId}", request, cancellationToken);
        await EnsureSuccessWithDetailsAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<InvItemCategory>(cancellationToken)
               ?? throw new InvalidOperationException("No item category returned by API.");
    }

    public async Task<bool> UpdateItemCategoryAsync(int id, UpdateInvItemCategoryRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"api/Inventory/InvItemCategories/{id}?viewerTenantId={viewerTenantId}", request, cancellationToken);
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

    public async Task<bool> DeleteItemCategoryAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"api/Inventory/InvItemCategories/{id}?viewerTenantId={viewerTenantId}", cancellationToken);
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
            throw new InvalidOperationException($"Item category API error ({(int)response.StatusCode}): {details}");
        }

        response.EnsureSuccessStatusCode();
    }
}

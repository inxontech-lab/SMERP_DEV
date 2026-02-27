using System.Net.Http.Json;
using Domain.InvReqDTO;
using Domain.SaasDBModels;

namespace Shared.Services.InventoryServices;

public interface IInvItemSubCategoryApiClient
{
    Task<List<InvItemSubCategory>> GetItemSubCategoriesAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<InvItemSubCategory> CreateItemSubCategoryAsync(CreateInvItemSubCategoryRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> UpdateItemSubCategoryAsync(int id, UpdateInvItemSubCategoryRequest request, int viewerTenantId, CancellationToken cancellationToken = default);
    Task<bool> DeleteItemSubCategoryAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default);
}

public class InvItemSubCategoryApiClient(HttpClient httpClient) : IInvItemSubCategoryApiClient
{
    public async Task<List<InvItemSubCategory>> GetItemSubCategoriesAsync(int viewerTenantId, CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<InvItemSubCategory>>($"api/Inventory/InvItemSubCategories?viewerTenantId={viewerTenantId}", cancellationToken) ?? [];

    public async Task<InvItemSubCategory> CreateItemSubCategoryAsync(CreateInvItemSubCategoryRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync($"api/Inventory/InvItemSubCategories?viewerTenantId={viewerTenantId}", request, cancellationToken);
        await EnsureSuccessWithDetailsAsync(response, cancellationToken);

        return await response.Content.ReadFromJsonAsync<InvItemSubCategory>(cancellationToken)
               ?? throw new InvalidOperationException("No item sub category returned by API.");
    }

    public async Task<bool> UpdateItemSubCategoryAsync(int id, UpdateInvItemSubCategoryRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PutAsJsonAsync($"api/Inventory/InvItemSubCategories/{id}?viewerTenantId={viewerTenantId}", request, cancellationToken);
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

    public async Task<bool> DeleteItemSubCategoryAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.DeleteAsync($"api/Inventory/InvItemSubCategories/{id}?viewerTenantId={viewerTenantId}", cancellationToken);
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
            throw new InvalidOperationException($"Item sub category API error ({(int)response.StatusCode}): {details}");
        }

        response.EnsureSuccessStatusCode();
    }
}

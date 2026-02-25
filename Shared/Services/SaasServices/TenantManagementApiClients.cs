using System.Net.Http.Json;
using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace Shared.Services.SaasServices;

public interface ITenantManagementApiClient
{
    Task<List<Tenant>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Tenant?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Tenant> CreateAsync(TenantRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, TenantRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public class TenantManagementApiClient(HttpClient httpClient) : ITenantManagementApiClient
{
    public async Task<List<Tenant>> GetAllAsync(CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<Tenant>>("api/Tenants", cancellationToken) ?? [];

    public async Task<Tenant?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<Tenant>($"api/Tenants/{id}", cancellationToken);

    public async Task<Tenant> CreateAsync(TenantRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/Tenants", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Tenant>(cancellationToken)
               ?? throw new InvalidOperationException("No Tenant returned by API.");
    }

    public async Task<bool> UpdateAsync(int id, TenantRequest request, CancellationToken cancellationToken = default)
        => (await httpClient.PutAsJsonAsync($"api/Tenants/{id}", request, cancellationToken)).IsSuccessStatusCode;

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        => (await httpClient.DeleteAsync($"api/Tenants/{id}", cancellationToken)).IsSuccessStatusCode;
}

public interface ITenantSettingManagementApiClient
{
    Task<List<TenantSetting>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TenantSetting?> GetByIdAsync(int tenantId, CancellationToken cancellationToken = default);
    Task<TenantSetting> CreateAsync(TenantSettingRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int tenantId, TenantSettingRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int tenantId, CancellationToken cancellationToken = default);
}

public class TenantSettingManagementApiClient(HttpClient httpClient) : ITenantSettingManagementApiClient
{
    public async Task<List<TenantSetting>> GetAllAsync(CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<TenantSetting>>("api/TenantSettings", cancellationToken) ?? [];

    public async Task<TenantSetting?> GetByIdAsync(int tenantId, CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<TenantSetting>($"api/TenantSettings/{tenantId}", cancellationToken);

    public async Task<TenantSetting> CreateAsync(TenantSettingRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/TenantSettings", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TenantSetting>(cancellationToken)
               ?? throw new InvalidOperationException("No TenantSetting returned by API.");
    }

    public async Task<bool> UpdateAsync(int tenantId, TenantSettingRequest request, CancellationToken cancellationToken = default)
        => (await httpClient.PutAsJsonAsync($"api/TenantSettings/{tenantId}", request, cancellationToken)).IsSuccessStatusCode;

    public async Task<bool> DeleteAsync(int tenantId, CancellationToken cancellationToken = default)
        => (await httpClient.DeleteAsync($"api/TenantSettings/{tenantId}", cancellationToken)).IsSuccessStatusCode;
}

using System.Net.Http.Json;

namespace SMERP.services.SaasServices;

public sealed class CrudApiClient<TEntity> : ICrudApiClient<TEntity> where TEntity : class
{
    private readonly HttpClient _httpClient;
    private readonly string _endpoint;

    public CrudApiClient(HttpClient httpClient, string endpoint)
    {
        _httpClient = httpClient;
        _endpoint = endpoint;
    }

    public Task<IReadOnlyList<TEntity>?> GetAllAsync(CancellationToken cancellationToken = default) =>
        _httpClient.GetFromJsonAsync<IReadOnlyList<TEntity>>(_endpoint, cancellationToken);

    public Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _httpClient.GetFromJsonAsync<TEntity>($"{_endpoint}/{id}", cancellationToken);

    public async Task<TEntity?> CreateAsync(TEntity model, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(_endpoint, model, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TEntity>(cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(int id, TEntity model, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"{_endpoint}/{id}", model, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"{_endpoint}/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}

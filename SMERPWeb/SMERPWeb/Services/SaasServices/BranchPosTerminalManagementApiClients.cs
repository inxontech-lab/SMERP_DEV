using System.Net.Http.Json;
using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPWeb.Services.SaasServices;

public interface IBranchManagementApiClient
{
    Task<List<Branch>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Branch?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Branch> CreateAsync(BranchRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, BranchRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public class BranchManagementApiClient(HttpClient httpClient) : IBranchManagementApiClient
{
    public async Task<List<Branch>> GetAllAsync(CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<Branch>>("api/Branches", cancellationToken) ?? [];

    public async Task<Branch?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<Branch>($"api/Branches/{id}", cancellationToken);

    public async Task<Branch> CreateAsync(BranchRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/Branches", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Branch>(cancellationToken)
               ?? throw new InvalidOperationException("No Branch returned by API.");
    }

    public async Task<bool> UpdateAsync(int id, BranchRequest request, CancellationToken cancellationToken = default)
        => (await httpClient.PutAsJsonAsync($"api/Branches/{id}", request, cancellationToken)).IsSuccessStatusCode;

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        => (await httpClient.DeleteAsync($"api/Branches/{id}", cancellationToken)).IsSuccessStatusCode;
}

public interface IPosTerminalManagementApiClient
{
    Task<List<PosTerminal>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PosTerminal?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PosTerminal> CreateAsync(PosTerminalRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, PosTerminalRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public class PosTerminalManagementApiClient(HttpClient httpClient) : IPosTerminalManagementApiClient
{
    public async Task<List<PosTerminal>> GetAllAsync(CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<PosTerminal>>("api/PosTerminals", cancellationToken) ?? [];

    public async Task<PosTerminal?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<PosTerminal>($"api/PosTerminals/{id}", cancellationToken);

    public async Task<PosTerminal> CreateAsync(PosTerminalRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/PosTerminals", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PosTerminal>(cancellationToken)
               ?? throw new InvalidOperationException("No POS Terminal returned by API.");
    }

    public async Task<bool> UpdateAsync(int id, PosTerminalRequest request, CancellationToken cancellationToken = default)
        => (await httpClient.PutAsJsonAsync($"api/PosTerminals/{id}", request, cancellationToken)).IsSuccessStatusCode;

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        => (await httpClient.DeleteAsync($"api/PosTerminals/{id}", cancellationToken)).IsSuccessStatusCode;
}

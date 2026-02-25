using System.Net.Http.Json;
using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace Shared.Services.SaasServices;

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

public interface IRoleManagementApiClient
{
    Task<List<Role>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Role> CreateAsync(RoleRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int id, RoleRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public class RoleManagementApiClient(HttpClient httpClient) : IRoleManagementApiClient
{
    public async Task<List<Role>> GetAllAsync(CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<Role>>("api/Roles", cancellationToken) ?? [];

    public async Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<Role>($"api/Roles/{id}", cancellationToken);

    public async Task<Role> CreateAsync(RoleRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/Roles", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Role>(cancellationToken)
               ?? throw new InvalidOperationException("No Role returned by API.");
    }

    public async Task<bool> UpdateAsync(int id, RoleRequest request, CancellationToken cancellationToken = default)
        => (await httpClient.PutAsJsonAsync($"api/Roles/{id}", request, cancellationToken)).IsSuccessStatusCode;

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        => (await httpClient.DeleteAsync($"api/Roles/{id}", cancellationToken)).IsSuccessStatusCode;
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

public interface IRolePermissionManagementApiClient
{
    Task<List<RolePermission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RolePermission?> GetByIdAsync(int tenantId, int roleId, int permissionId, CancellationToken cancellationToken = default);
    Task<RolePermission> CreateAsync(RolePermissionRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int tenantId, int roleId, int permissionId, RolePermissionRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int tenantId, int roleId, int permissionId, CancellationToken cancellationToken = default);
}

public class RolePermissionManagementApiClient(HttpClient httpClient) : IRolePermissionManagementApiClient
{
    public async Task<List<RolePermission>> GetAllAsync(CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<RolePermission>>("api/RolePermissions", cancellationToken) ?? [];

    public async Task<RolePermission?> GetByIdAsync(int tenantId, int roleId, int permissionId, CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<RolePermission>($"api/RolePermissions/{tenantId}/{roleId}/{permissionId}", cancellationToken);

    public async Task<RolePermission> CreateAsync(RolePermissionRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/RolePermissions", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<RolePermission>(cancellationToken)
               ?? throw new InvalidOperationException("No RolePermission returned by API.");
    }

    public async Task<bool> UpdateAsync(int tenantId, int roleId, int permissionId, RolePermissionRequest request, CancellationToken cancellationToken = default)
        => (await httpClient.PutAsJsonAsync($"api/RolePermissions/{tenantId}/{roleId}/{permissionId}", request, cancellationToken)).IsSuccessStatusCode;

    public async Task<bool> DeleteAsync(int tenantId, int roleId, int permissionId, CancellationToken cancellationToken = default)
        => (await httpClient.DeleteAsync($"api/RolePermissions/{tenantId}/{roleId}/{permissionId}", cancellationToken)).IsSuccessStatusCode;
}

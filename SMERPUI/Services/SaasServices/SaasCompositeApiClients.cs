using System.Net.Http.Json;
using Domain.SaasDBModels;

namespace SMERPUI.Services.SaasServices;

public interface IUserRoleApiClient
{
    Task<List<UserRole>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserRole?> GetByIdAsync(int tenantId, long userId, int roleId, CancellationToken cancellationToken = default);
    Task<UserRole> CreateAsync(UserRole entity, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int tenantId, long userId, int roleId, UserRole entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int tenantId, long userId, int roleId, CancellationToken cancellationToken = default);
}

public class UserRoleApiClient(HttpClient httpClient) : IUserRoleApiClient
{
    public async Task<List<UserRole>> GetAllAsync(CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<UserRole>>("api/UserRoles", cancellationToken) ?? [];

    public async Task<UserRole?> GetByIdAsync(int tenantId, long userId, int roleId, CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<UserRole>($"api/UserRoles/{tenantId}/{userId}/{roleId}", cancellationToken);

    public async Task<UserRole> CreateAsync(UserRole entity, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/UserRoles", entity, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UserRole>(cancellationToken)
               ?? throw new InvalidOperationException("No UserRole returned by API.");
    }

    public async Task<bool> UpdateAsync(int tenantId, long userId, int roleId, UserRole entity, CancellationToken cancellationToken = default)
        => (await httpClient.PutAsJsonAsync($"api/UserRoles/{tenantId}/{userId}/{roleId}", entity, cancellationToken)).IsSuccessStatusCode;

    public async Task<bool> DeleteAsync(int tenantId, long userId, int roleId, CancellationToken cancellationToken = default)
        => (await httpClient.DeleteAsync($"api/UserRoles/{tenantId}/{userId}/{roleId}", cancellationToken)).IsSuccessStatusCode;
}

public interface IUserBranchApiClient
{
    Task<List<UserBranch>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserBranch?> GetByIdAsync(int tenantId, long userId, int branchId, CancellationToken cancellationToken = default);
    Task<UserBranch> CreateAsync(UserBranch entity, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int tenantId, long userId, int branchId, UserBranch entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int tenantId, long userId, int branchId, CancellationToken cancellationToken = default);
}

public class UserBranchApiClient(HttpClient httpClient) : IUserBranchApiClient
{
    public async Task<List<UserBranch>> GetAllAsync(CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<UserBranch>>("api/UserBranches", cancellationToken) ?? [];

    public async Task<UserBranch?> GetByIdAsync(int tenantId, long userId, int branchId, CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<UserBranch>($"api/UserBranches/{tenantId}/{userId}/{branchId}", cancellationToken);

    public async Task<UserBranch> CreateAsync(UserBranch entity, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/UserBranches", entity, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UserBranch>(cancellationToken)
               ?? throw new InvalidOperationException("No UserBranch returned by API.");
    }

    public async Task<bool> UpdateAsync(int tenantId, long userId, int branchId, UserBranch entity, CancellationToken cancellationToken = default)
        => (await httpClient.PutAsJsonAsync($"api/UserBranches/{tenantId}/{userId}/{branchId}", entity, cancellationToken)).IsSuccessStatusCode;

    public async Task<bool> DeleteAsync(int tenantId, long userId, int branchId, CancellationToken cancellationToken = default)
        => (await httpClient.DeleteAsync($"api/UserBranches/{tenantId}/{userId}/{branchId}", cancellationToken)).IsSuccessStatusCode;
}

public interface IRolePermissionApiClient
{
    Task<List<RolePermission>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RolePermission?> GetByIdAsync(int tenantId, int roleId, int permissionId, CancellationToken cancellationToken = default);
    Task<RolePermission> CreateAsync(RolePermission entity, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(int tenantId, int roleId, int permissionId, RolePermission entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int tenantId, int roleId, int permissionId, CancellationToken cancellationToken = default);
}

public class RolePermissionApiClient(HttpClient httpClient) : IRolePermissionApiClient
{
    public async Task<List<RolePermission>> GetAllAsync(CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<RolePermission>>("api/RolePermissions", cancellationToken) ?? [];

    public async Task<RolePermission?> GetByIdAsync(int tenantId, int roleId, int permissionId, CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<RolePermission>($"api/RolePermissions/{tenantId}/{roleId}/{permissionId}", cancellationToken);

    public async Task<RolePermission> CreateAsync(RolePermission entity, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/RolePermissions", entity, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<RolePermission>(cancellationToken)
               ?? throw new InvalidOperationException("No RolePermission returned by API.");
    }

    public async Task<bool> UpdateAsync(int tenantId, int roleId, int permissionId, RolePermission entity, CancellationToken cancellationToken = default)
        => (await httpClient.PutAsJsonAsync($"api/RolePermissions/{tenantId}/{roleId}/{permissionId}", entity, cancellationToken)).IsSuccessStatusCode;

    public async Task<bool> DeleteAsync(int tenantId, int roleId, int permissionId, CancellationToken cancellationToken = default)
        => (await httpClient.DeleteAsync($"api/RolePermissions/{tenantId}/{roleId}/{permissionId}", cancellationToken)).IsSuccessStatusCode;
}

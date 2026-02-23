using System.Net.Http.Json;
using Domain.SaasDBModels;

namespace SMERPUI.Services.SaasServices;

public interface ISaasCrudApiClient<TEntity, in TKey> where TEntity : class
{
    Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> UpdateAsync(TKey id, TEntity entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default);
}

public class SaasCrudApiClient<TEntity, TKey> : ISaasCrudApiClient<TEntity, TKey> where TEntity : class
{
    private readonly HttpClient _httpClient;
    private readonly string _endpoint;

    protected SaasCrudApiClient(HttpClient httpClient, string endpoint)
    {
        _httpClient = httpClient;
        _endpoint = endpoint.Trim('/');
    }

    public async Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<List<TEntity>>($"api/{_endpoint}", cancellationToken) ?? [];
    }

    public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<TEntity>($"api/{_endpoint}/{id}", cancellationToken);
    }

    public async Task<TEntity> CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/{_endpoint}", entity, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TEntity>(cancellationToken)
               ?? throw new InvalidOperationException($"No {typeof(TEntity).Name} returned by API.");
    }

    public async Task<bool> UpdateAsync(TKey id, TEntity entity, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/{_endpoint}/{id}", entity, cancellationToken);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"api/{_endpoint}/{id}", cancellationToken);
        return response.IsSuccessStatusCode;
    }
}

public interface IAuditLogApiClient : ISaasCrudApiClient<AuditLog, long>;
public class AuditLogApiClient(HttpClient httpClient) : SaasCrudApiClient<AuditLog, long>(httpClient, "AuditLogs"), IAuditLogApiClient;

public interface IBranchApiClient : ISaasCrudApiClient<Branch, int>;
public class BranchApiClient(HttpClient httpClient) : SaasCrudApiClient<Branch, int>(httpClient, "Branches"), IBranchApiClient;

public interface IPermissionApiClient : ISaasCrudApiClient<Permission, int>;
public class PermissionApiClient(HttpClient httpClient) : SaasCrudApiClient<Permission, int>(httpClient, "Permissions"), IPermissionApiClient;

public interface IPosTerminalApiClient : ISaasCrudApiClient<PosTerminal, int>;
public class PosTerminalApiClient(HttpClient httpClient) : SaasCrudApiClient<PosTerminal, int>(httpClient, "PosTerminals"), IPosTerminalApiClient;
public interface IRefreshTokenApiClient : ISaasCrudApiClient<RefreshToken, long>;
public class RefreshTokenApiClient(HttpClient httpClient) : SaasCrudApiClient<RefreshToken, long>(httpClient, "RefreshTokens"), IRefreshTokenApiClient;

public interface IRoleApiClient : ISaasCrudApiClient<Role, int>;
public class RoleApiClient(HttpClient httpClient) : SaasCrudApiClient<Role, int>(httpClient, "Roles"), IRoleApiClient;

public interface ITaxCodeApiClient : ISaasCrudApiClient<TaxCode, int>;
public class TaxCodeApiClient(HttpClient httpClient) : SaasCrudApiClient<TaxCode, int>(httpClient, "TaxCodes"), ITaxCodeApiClient;

public interface ITenantApiClient : ISaasCrudApiClient<Tenant, int>;
public class TenantApiClient(HttpClient httpClient) : SaasCrudApiClient<Tenant, int>(httpClient, "Tenants"), ITenantApiClient;

public interface ITenantSettingApiClient : ISaasCrudApiClient<TenantSetting, int>;
public class TenantSettingApiClient(HttpClient httpClient) : SaasCrudApiClient<TenantSetting, int>(httpClient, "TenantSettings"), ITenantSettingApiClient;

public interface IUomApiClient : ISaasCrudApiClient<Uom, int>;
public class UomApiClient(HttpClient httpClient) : SaasCrudApiClient<Uom, int>(httpClient, "Uoms"), IUomApiClient;

public interface IUserApiClient : ISaasCrudApiClient<User, long>;
public class UserApiClient(HttpClient httpClient) : SaasCrudApiClient<User, long>(httpClient, "Users"), IUserApiClient;

using System.Net.Http.Json;
using Domain.SaasReqDTO;

namespace Shared.Services.SaasServices;

public interface IUserManagementApiClient
{
    Task<List<UserWithRoleResponse>> GetUsersAsync(int viewerTenantId, CancellationToken cancellationToken = default);
    Task<long> CreateUserAsync(CreateUserWithRoleRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserAsync(long userId, UpdateUserWithRoleRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(long userId, CancellationToken cancellationToken = default);
}

public class UserManagementApiClient(HttpClient httpClient) : IUserManagementApiClient
{
    public async Task<List<UserWithRoleResponse>> GetUsersAsync(int viewerTenantId, CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<UserWithRoleResponse>>($"api/Users/with-role?viewerTenantId={viewerTenantId}", cancellationToken) ?? [];

    public async Task<long> CreateUserAsync(CreateUserWithRoleRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/Users/with-role", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var userId = await response.Content.ReadFromJsonAsync<long?>(cancellationToken);
        return userId ?? throw new InvalidOperationException("No user id returned by API.");
    }

    public async Task<bool> UpdateUserAsync(long userId, UpdateUserWithRoleRequest request, CancellationToken cancellationToken = default)
        => (await httpClient.PutAsJsonAsync($"api/Users/with-role/{userId}", request, cancellationToken)).IsSuccessStatusCode;

    public async Task<bool> DeleteUserAsync(long userId, CancellationToken cancellationToken = default)
        => (await httpClient.DeleteAsync($"api/Users/with-role/{userId}", cancellationToken)).IsSuccessStatusCode;
}

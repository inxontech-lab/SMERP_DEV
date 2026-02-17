using System.Net.Http.Json;
using Domain.SaasReqDTO;

namespace SMERPWeb.Services.SaasServices;

public interface IUserOnboardingApiClient
{
    Task<List<UserWithRoleResponse>> GetUsersWithRolesAsync(CancellationToken cancellationToken = default);
    Task<long> CreateUserWithRoleAsync(CreateUserWithRoleRequest request, CancellationToken cancellationToken = default);
    Task<bool> UpdateUserWithRoleAsync(long userId, UpdateUserWithRoleRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserWithRoleAsync(long userId, CancellationToken cancellationToken = default);
}

public class UserOnboardingApiClient(HttpClient httpClient) : IUserOnboardingApiClient
{
    public async Task<List<UserWithRoleResponse>> GetUsersWithRolesAsync(CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<UserWithRoleResponse>>("api/Users/with-role", cancellationToken) ?? [];

    public async Task<long> CreateUserWithRoleAsync(CreateUserWithRoleRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/Users/with-role", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var userId = await response.Content.ReadFromJsonAsync<long?>(cancellationToken);
        return userId ?? throw new InvalidOperationException("No user id returned by API.");
    }

    public async Task<bool> UpdateUserWithRoleAsync(long userId, UpdateUserWithRoleRequest request, CancellationToken cancellationToken = default)
        => (await httpClient.PutAsJsonAsync($"api/Users/with-role/{userId}", request, cancellationToken)).IsSuccessStatusCode;

    public async Task<bool> DeleteUserWithRoleAsync(long userId, CancellationToken cancellationToken = default)
        => (await httpClient.DeleteAsync($"api/Users/with-role/{userId}", cancellationToken)).IsSuccessStatusCode;
}

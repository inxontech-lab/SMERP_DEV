using System.Net.Http.Json;
using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPWeb.Services.SaasServices;

public interface IUserRequestApiClient
{
    Task<User> CreateAsync(UserRequest request, CancellationToken cancellationToken = default);
}

public class UserRequestApiClient(HttpClient httpClient) : IUserRequestApiClient
{
    public async Task<User> CreateAsync(UserRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/Users", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<User>(cancellationToken)
               ?? throw new InvalidOperationException("No User returned by API.");
    }
}

public interface IUserRoleRequestApiClient
{
    Task<UserRole> CreateAsync(UserRoleRequest request, CancellationToken cancellationToken = default);
}

public class UserRoleRequestApiClient(HttpClient httpClient) : IUserRoleRequestApiClient
{
    public async Task<UserRole> CreateAsync(UserRoleRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/UserRoles", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<UserRole>(cancellationToken)
               ?? throw new InvalidOperationException("No UserRole returned by API.");
    }
}

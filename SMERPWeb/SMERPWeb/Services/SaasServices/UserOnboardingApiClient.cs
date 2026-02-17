using System.Net.Http.Json;
using Domain.SaasReqDTO;

namespace SMERPWeb.Services.SaasServices;

public interface IUserOnboardingApiClient
{
    Task<long> CreateUserWithRoleAsync(CreateUserWithRoleRequest request, CancellationToken cancellationToken = default);
}

public class UserOnboardingApiClient(HttpClient httpClient) : IUserOnboardingApiClient
{
    public async Task<long> CreateUserWithRoleAsync(CreateUserWithRoleRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/Users/with-role", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<long>(cancellationToken);
    }
}

using System.Net.Http.Json;

namespace SMERPWeb.Services.SaasServices;

public interface IAuthApiClient
{
    Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}

public class AuthApiClient(HttpClient httpClient) : IAuthApiClient
{
    public async Task<LoginResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.PostAsJsonAsync("api/Auth/login", request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        return await response.Content.ReadFromJsonAsync<LoginResponse>(cancellationToken);
    }
}

public record LoginRequest(string TenantEmail, string Username, string Password);

public record LoginResponse(
    long Id,
    int TenantId,
    string Username,
    string DisplayName,
    string? Email,
    string? Mobile,
    DateTime? LastLoginAt);

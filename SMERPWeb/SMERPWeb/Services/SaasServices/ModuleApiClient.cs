using System.Net.Http.Json;

namespace SMERPWeb.Services.SaasServices;

public sealed record PermittedModuleDto(string ModuleKey, string ModuleName);

public interface IModuleApiClient
{
    Task<List<PermittedModuleDto>> GetPermittedModulesAsync(int tenantId, long userId, CancellationToken cancellationToken = default);
}

public class ModuleApiClient(HttpClient httpClient) : IModuleApiClient
{
    public async Task<List<PermittedModuleDto>> GetPermittedModulesAsync(int tenantId, long userId, CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<PermittedModuleDto>>($"api/Modules/permitted/{tenantId}/{userId}", cancellationToken) ?? [];
}

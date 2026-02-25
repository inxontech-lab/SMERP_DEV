using System.Net.Http.Json;

namespace Shared.Services.SaasServices;

public sealed record PermittedModuleDto(string ModuleKey, string ModuleName);
public sealed record NavigationMenuSnapshotDto(
    IReadOnlyList<PermittedModuleDto> Modules,
    IReadOnlyList<NavigationMenuGroupDto> MenuGroups,
    IReadOnlyList<string> AllowedPaths);

public sealed record NavigationMenuGroupDto(string MainMenu, IReadOnlyList<NavigationMenuItemDto> Items);

public sealed record NavigationMenuItemDto(
    string Text,
    string Path,
    string Icon,
    string ModuleKey,
    string MainMenu,
    string SubMenu,
    IReadOnlyList<string> PermissionKeywords);

public interface IModuleApiClient
{
    Task<List<PermittedModuleDto>> GetPermittedModulesAsync(int tenantId, long userId, CancellationToken cancellationToken = default);
}

public interface IMenuApiClient
{
    Task<NavigationMenuSnapshotDto> GetNavigationMenuAsync(int tenantId, long userId, string? moduleKey, CancellationToken cancellationToken = default);
}

public class ModuleApiClient(HttpClient httpClient) : IModuleApiClient
{
    public async Task<List<PermittedModuleDto>> GetPermittedModulesAsync(int tenantId, long userId, CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonAsync<List<PermittedModuleDto>>($"api/Modules/permitted/{tenantId}/{userId}", cancellationToken) ?? [];
}

public class MenuApiClient(HttpClient httpClient) : IMenuApiClient
{
    public async Task<NavigationMenuSnapshotDto> GetNavigationMenuAsync(int tenantId, long userId, string? moduleKey, CancellationToken cancellationToken = default)
    {
        var encodedModuleKey = string.IsNullOrWhiteSpace(moduleKey) ? string.Empty : $"?moduleKey={Uri.EscapeDataString(moduleKey)}";
        return await httpClient.GetFromJsonAsync<NavigationMenuSnapshotDto>($"api/Menus/navigation/{tenantId}/{userId}{encodedModuleKey}", cancellationToken)
               ?? new NavigationMenuSnapshotDto([], [], []);
    }
}

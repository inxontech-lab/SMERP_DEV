using Microsoft.JSInterop;
using SMERPWeb.Services.Auth;
using SMERPWeb.Services.Interop;
using SMERPWeb.Services.SaasServices;

namespace SMERPWeb.Services.Navigation;

public sealed record ModuleCardDefinition(string Key, string Name, string Icon, string Description, string DefaultPath);
public sealed record MenuGroupDefinition(string GroupName, IReadOnlyList<MenuItemDefinition> Items);
public sealed record MenuItemDefinition(string Text, string Path, string Icon, string ModuleKey, string SubMenu, IReadOnlyList<string> PermissionKeywords);
public sealed record NavigationSnapshot(
    IReadOnlyList<ModuleCardDefinition> Modules,
    IReadOnlyList<MenuGroupDefinition> MenuGroups,
    string? SelectedModule,
    bool HasAnyMenuItems,
    IReadOnlySet<string> AllowedPaths);

public interface INavigationCatalogService
{
    Task<NavigationSnapshot> BuildSnapshotAsync(UserSession session, CancellationToken cancellationToken = default);
    Task SetSelectedModuleAsync(string moduleKey);
}

public class NavigationCatalogService(
    IMenuApiClient menuApiClient,
    IJSRuntime jsRuntime) : INavigationCatalogService
{
    private const string SelectedModuleStorageKey = "smerp-selected-module";

    public async Task<NavigationSnapshot> BuildSnapshotAsync(UserSession session, CancellationToken cancellationToken = default)
    {
        var storedModule = await TryGetSelectedModuleAsync();
        var requestedModule = !string.IsNullOrWhiteSpace(storedModule) ? storedModule : session.SelectedModule;

        var menuSnapshot = await menuApiClient.GetNavigationMenuAsync(session.TenantId, session.UserId, requestedModule, cancellationToken);
        var modules = menuSnapshot.Modules
            .Select(module =>
                new ModuleCardDefinition(
                    module.ModuleKey,
                    module.ModuleName,
                    ResolveIcon(module.ModuleName),
                    ResolveDescription(module.ModuleName),
                    "/Home"))
            .DistinctBy(module => module.Key)
            .ToList();

        var selectedModule = ResolveSelectedModule(session.SelectedModule, storedModule, modules);
        var menuGroups = menuSnapshot.MenuGroups
            .Select(group =>
                new MenuGroupDefinition(
                    group.MainMenu,
                    group.Items.Select(item => new MenuItemDefinition(item.Text, item.Path, item.Icon, item.ModuleKey, item.SubMenu, item.PermissionKeywords)).ToList()))
            .ToList();

        var allowedPaths = menuSnapshot.AllowedPaths
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (allowedPaths.Count == 0)
        {
            allowedPaths.Add("/Home");
        }

        return new NavigationSnapshot(modules, menuGroups, selectedModule, allowedPaths.Count > 0, allowedPaths);
    }

    public async Task SetSelectedModuleAsync(string moduleKey)
    {
        try
        {
            await jsRuntime.InvokeVoidAsync("sessionManager.set", SelectedModuleStorageKey, moduleKey);
        }
        catch (Exception ex) when (JsInteropGuard.IsUnavailable(ex))
        {
            // Ignore during static pre-render.
        }
    }


    private async Task<string?> TryGetSelectedModuleAsync()
    {
        try
        {
            return await jsRuntime.InvokeAsync<string?>("sessionManager.get", SelectedModuleStorageKey);
        }
        catch (Exception ex) when (JsInteropGuard.IsUnavailable(ex))
        {
            return null;
        }
    }

    private static string ResolveIcon(string moduleName)
    {
        var value = moduleName.Trim().ToLowerInvariant();
        return value switch
        {
            var name when name.Contains("master") => "folder",
            var name when name.Contains("trans") || name.Contains("sales") => "receipt_long",
            var name when name.Contains("account") || name.Contains("finance") => "account_balance_wallet",
            var name when name.Contains("report") => "bar_chart",
            var name when name.Contains("inventory") || name.Contains("stock") => "inventory_2",
            _ => "apps"
        };
    }

    private static string ResolveDescription(string moduleName)
    {
        var value = moduleName.Trim().ToLowerInvariant();
        return value switch
        {
            var name when name.Contains("master") => "Configure foundational tenant setup and controls.",
            var name when name.Contains("trans") || name.Contains("sales") => "Execute daily operational and business transactions.",
            var name when name.Contains("account") || name.Contains("finance") => "Manage accounting flows, ledgers, and balances.",
            var name when name.Contains("report") => "Review insights and analytics for your workspace.",
            var name when name.Contains("inventory") || name.Contains("stock") => "Track inventory movement and availability.",
            _ => "Access this workspace module based on your tenant permissions."
        };
    }

    private static string? ResolveSelectedModule(string? sessionSelectedModule, string? storedModule, IReadOnlyCollection<ModuleCardDefinition> modules)
    {
        if (!string.IsNullOrWhiteSpace(storedModule) && modules.Any(module => module.Key == storedModule))
        {
            return storedModule;
        }

        if (!string.IsNullOrWhiteSpace(sessionSelectedModule) && modules.Any(module => module.Key == sessionSelectedModule))
        {
            return sessionSelectedModule;
        }

        return modules.FirstOrDefault()?.Key;
    }
}

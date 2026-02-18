using Domain.SaasDBModels;
using Microsoft.JSInterop;
using SMERPWeb.Services.Auth;
using SMERPWeb.Services.SaasServices;

namespace SMERPWeb.Services.Navigation;

public sealed record ModuleCardDefinition(string Key, string Name, string Icon, string Description, string DefaultPath);
public sealed record MenuGroupDefinition(string GroupName, IReadOnlyList<MenuItemDefinition> Items);
public sealed record MenuItemDefinition(string Text, string Path, string Icon, string ModuleKey, IReadOnlyList<string> PermissionKeywords);
public sealed record NavigationSnapshot(
    IReadOnlyList<ModuleCardDefinition> Modules,
    IReadOnlyList<MenuGroupDefinition> MenuGroups,
    string? SelectedModule,
    bool HasAnyMenuItems);

public interface INavigationCatalogService
{
    Task<NavigationSnapshot> BuildSnapshotAsync(UserSession session, CancellationToken cancellationToken = default);
    Task SetSelectedModuleAsync(string moduleKey);
}

public class NavigationCatalogService(
    IModuleApiClient moduleApiClient,
    IUserRoleApiClient userRoleApiClient,
    IRolePermissionApiClient rolePermissionApiClient,
    IPermissionApiClient permissionApiClient,
    IJSRuntime jsRuntime) : INavigationCatalogService
{
    private const string SelectedModuleStorageKey = "smerp-selected-module";

    private static readonly IReadOnlyList<MenuItemDefinition> MenuCatalog =
    [
        new("Home", "/Home", "home", "master", ["dashboard", "home", "view"]),
        new("Tenant", "/Tenant", "business", "master", ["tenant"]),
        new("Tenant Setting", "/TenantSetting", "settings", "master", ["tenant_setting", "tenantsetting", "setting"]),
        new("Branch", "/Branch", "account_tree", "master", ["branch"]),
        new("Role", "/Role", "badge", "master", ["role"]),
        new("Role Permission", "/RolePermission", "lock_open", "master", ["role_permission", "permission"]),
        new("POS Terminal", "/PosTerminal", "point_of_sale", "master", ["pos", "terminal"]),
        new("User", "/User", "account_circle", "master", ["user"])
    ];

    public async Task<NavigationSnapshot> BuildSnapshotAsync(UserSession session, CancellationToken cancellationToken = default)
    {
        var modules = await ResolvePermittedModulesAsync(session, cancellationToken);

        var permissions = await ResolvePermissionsAsync(session, cancellationToken);
        var permissionTokens = permissions
            .SelectMany(p => new[] { p.Code, p.Name, p.Module ?? string.Empty })
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim().ToLowerInvariant())
            .ToHashSet();

        var storedModule = await jsRuntime.InvokeAsync<string?>("sessionManager.get", SelectedModuleStorageKey);
        var selectedModule = ResolveSelectedModule(session.SelectedModule, storedModule, modules);

        var filteredItems = MenuCatalog
            .Where(item => item.ModuleKey == selectedModule)
            .Where(item => IsMenuItemAllowed(item, permissionTokens))
            .ToList();

        if (selectedModule is not null && filteredItems.Count == 0)
        {
            filteredItems = MenuCatalog.Where(item => item.ModuleKey == selectedModule).ToList();
        }

        IReadOnlyList<MenuGroupDefinition> menuGroups = filteredItems.Count == 0
            ? Array.Empty<MenuGroupDefinition>()
            : new[] { new MenuGroupDefinition("Navigation", filteredItems) };

        return new NavigationSnapshot(modules, menuGroups, selectedModule, filteredItems.Count > 0);
    }

    private async Task<List<ModuleCardDefinition>> ResolvePermittedModulesAsync(UserSession session, CancellationToken cancellationToken)
    {
        var modules = await moduleApiClient.GetPermittedModulesAsync(session.TenantId, session.UserId, cancellationToken);

        if (modules.Count == 0)
        {
            return [];
        }

        return modules
            .Select(module =>
            {
                var moduleName = string.IsNullOrWhiteSpace(module.ModuleName) ? module.ModuleKey : module.ModuleName;
                var moduleKey = string.IsNullOrWhiteSpace(module.ModuleKey) ? ResolveModuleKey(moduleName) : module.ModuleKey;
                return new ModuleCardDefinition(
                    moduleKey,
                    moduleName,
                    ResolveIcon(moduleName),
                    ResolveDescription(moduleName),
                    "/Home");
            })
            .DistinctBy(module => module.Key)
            .ToList();
    }

    public async Task SetSelectedModuleAsync(string moduleKey)
    {
        await jsRuntime.InvokeVoidAsync("sessionManager.set", SelectedModuleStorageKey, moduleKey);
    }

    private async Task<List<Permission>> ResolvePermissionsAsync(UserSession session, CancellationToken cancellationToken)
    {
        var userRoles = await userRoleApiClient.GetAllAsync(cancellationToken);
        var roleIds = userRoles
            .Where(role => role.TenantId == session.TenantId && role.UserId == session.UserId)
            .Select(role => role.RoleId)
            .Distinct()
            .ToHashSet();

        if (roleIds.Count == 0)
        {
            return [];
        }

        var rolePermissions = await rolePermissionApiClient.GetAllAsync(cancellationToken);
        var permissionIds = rolePermissions
            .Where(rolePermission => rolePermission.TenantId == session.TenantId && roleIds.Contains(rolePermission.RoleId))
            .Select(rolePermission => rolePermission.PermissionId)
            .Distinct()
            .ToHashSet();

        if (permissionIds.Count == 0)
        {
            return [];
        }

        var allPermissions = await permissionApiClient.GetAllAsync(cancellationToken);
        return allPermissions.Where(permission => permissionIds.Contains(permission.Id)).ToList();
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

    private static string ResolveModuleKey(string moduleName)
    {
        var value = moduleName.Trim().ToLowerInvariant();
        if (value.Contains("master")) return "master";
        if (value.Contains("trans") || value.Contains("sales")) return "transactions";
        if (value.Contains("account") || value.Contains("finance")) return "accounts";
        if (value.Contains("report")) return "reports";
        return value.Replace(" ", "-");
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

    private static bool IsMenuItemAllowed(MenuItemDefinition item, HashSet<string> permissionTokens)
    {
        if (permissionTokens.Count == 0)
        {
            return true;
        }

        return item.PermissionKeywords.Any(keyword =>
            permissionTokens.Any(token => token.Contains(keyword, StringComparison.OrdinalIgnoreCase)));
    }
}

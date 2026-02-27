using Domain.SaasDBModels;
using Microsoft.EntityFrameworkCore;

namespace SMERPAPIs.Services.SaasServices;

public interface IMenuService
{
    Task<NavigationMenuSnapshotDto> GetNavigationMenuAsync(int tenantId, long userId, string? moduleKey, CancellationToken cancellationToken = default);
}

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

public class MenuService(SmerpContext context) : IMenuService
{
    private static readonly IReadOnlyList<NavigationMenuItemDto> MenuCatalog =
    [
        new("Home", "/Home", "home", "admin", "Administration", "Home", ["dashboard", "home", "view"]),
        new("Tenant", "/Tenant", "business", "admin", "Administration", "Organization", ["tenant"]),
        new("Tenant Setting", "/TenantSetting", "settings", "admin", "Administration", "Organization", ["tenant_setting", "tenantsetting", "setting"]),
        new("Branch", "/Branch", "account_tree", "admin", "Administration", "Organization", ["branch"]),
        new("Role", "/Role", "badge", "admin", "Administration", "Security", ["role"]),
        new("Role Permission", "/RolePermission", "lock_open", "admin", "Administration", "Security", ["role_permission", "permission"]),
        new("User", "/User", "account_circle", "admin", "Administration", "Security", ["user"]),
        new("POS Terminal", "/PosTerminal", "point_of_sale", "admin", "Administration", "Operations", ["pos", "terminal"]),

        new("Supplier", "/Suppliers", "point_of_sale", "inventory", "Master Setup", "Supplier", ["supplier"]),
        new("Warehouse", "/Warehouses", "warehouse", "inventory", "Master Setup", "Warehouse", ["warehouse"]),
        new("UOM", "/Uoms", "warehouse", "inventory", "Master Setup", "Item Setup", ["uom"]),
        new("Item UOM Conversion", "/ItemUomConversions", "swap_horiz", "inventory", "Master Setup", "Item Setup", ["item_uom_conversion", "uom_conversion", "uom", "item"])
    ];

    public async Task<NavigationMenuSnapshotDto> GetNavigationMenuAsync(int tenantId, long userId, string? moduleKey, CancellationToken cancellationToken = default)
    {
        var permissions = await ResolvePermissionsAsync(tenantId, userId, cancellationToken);
        var permissionTokens = permissions
            .SelectMany(permission => new[] { permission.Code, permission.Name, permission.Module ?? string.Empty })
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim().ToLowerInvariant())
            .ToHashSet();

        var modules = permissions
            .Where(permission => !string.IsNullOrWhiteSpace(permission.Module))
            .Select(permission => permission.Module!.Trim())
            .Distinct()
            .OrderBy(value => value)
            .Select(moduleName => new PermittedModuleDto(ToModuleKey(moduleName), moduleName))
            .DistinctBy(module => module.ModuleKey)
            .ToList();

        var selectedModule = ResolveSelectedModule(moduleKey, modules);
        var filteredItems = MenuCatalog
            .Where(item => item.ModuleKey == selectedModule)
            .Where(item => IsMenuItemAllowed(item, permissionTokens))
            .ToList();

        if (selectedModule is not null && filteredItems.Count == 0)
        {
            filteredItems = MenuCatalog.Where(item => item.ModuleKey == selectedModule).ToList();
        }

        var groupedItems = filteredItems
            .GroupBy(item => item.MainMenu)
            .Select(group => new NavigationMenuGroupDto(group.Key, group.ToList()))
            .ToList();

        return new NavigationMenuSnapshotDto(modules, groupedItems, filteredItems.Select(item => item.Path).Distinct().ToList());
    }

    private async Task<List<Permission>> ResolvePermissionsAsync(int tenantId, long userId, CancellationToken cancellationToken)
    {
        return await (
            from userRole in context.UserRoles
            join role in context.Roles on new { userRole.TenantId, userRole.RoleId } equals new { role.TenantId, RoleId = role.Id }
            join rolePermission in context.RolePermissions on new { userRole.TenantId, userRole.RoleId } equals new { rolePermission.TenantId, rolePermission.RoleId }
            join permission in context.Permissions on rolePermission.PermissionId equals permission.Id
            where userRole.TenantId == tenantId
                  && userRole.UserId == userId
                  && role.IsActive
            select permission)
            .Distinct()
            .ToListAsync(cancellationToken);
    }

    private static string? ResolveSelectedModule(string? requestedModuleKey, IReadOnlyCollection<PermittedModuleDto> modules)
    {
        if (!string.IsNullOrWhiteSpace(requestedModuleKey) && modules.Any(module => module.ModuleKey == requestedModuleKey))
        {
            return requestedModuleKey;
        }

        return modules.FirstOrDefault()?.ModuleKey;
    }

    private static bool IsMenuItemAllowed(NavigationMenuItemDto item, HashSet<string> permissionTokens)
    {
        if (permissionTokens.Count == 0)
        {
            return true;
        }

        return item.PermissionKeywords.Any(keyword =>
            permissionTokens.Any(token => token.Contains(keyword, StringComparison.OrdinalIgnoreCase)));
    }

    private static string ToModuleKey(string moduleName)
    {
        var normalized = moduleName.Trim().ToLowerInvariant();

        if (normalized.Contains("master")) return "master";
        if (normalized.Contains("trans") || normalized.Contains("sales")) return "transactions";
        if (normalized.Contains("account") || normalized.Contains("finance")) return "accounts";
        if (normalized.Contains("report")) return "reports";

        return normalized.Replace(" ", "-");
    }
}

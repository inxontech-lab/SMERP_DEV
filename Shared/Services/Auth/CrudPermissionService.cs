using System.Globalization;
using Shared.Services.SaasServices;

namespace Shared.Services.Auth;

public interface ICrudPermissionService
{
    Task<CrudPermissionSnapshot> GetPermissionsAsync(string resourceKey, CancellationToken cancellationToken = default);
}

public sealed record CrudPermissionSnapshot(bool CanCreate, bool CanEdit, bool CanDelete)
{
    public static readonly CrudPermissionSnapshot None = new(false, false, false);
}

public sealed class CrudPermissionService(
    IUserSessionService userSessionService,
    IUserRoleApiClient userRoleApiClient,
    IRolePermissionApiClient rolePermissionApiClient,
    IPermissionApiClient permissionApiClient) : ICrudPermissionService
{
    public async Task<CrudPermissionSnapshot> GetPermissionsAsync(string resourceKey, CancellationToken cancellationToken = default)
    {
        var session = await userSessionService.GetSessionAsync();
        if (session is null || string.IsNullOrWhiteSpace(resourceKey))
        {
            return CrudPermissionSnapshot.None;
        }

        var normalizedResourceKey = NormalizeToken(resourceKey);
        var permissionCodes = await ResolvePermissionCodesAsync(session.TenantId, session.UserId, cancellationToken);

        return new CrudPermissionSnapshot(
            HasAnyPermission(permissionCodes, normalizedResourceKey, "create"),
            HasAnyPermission(permissionCodes, normalizedResourceKey, "edit", "update"),
            HasAnyPermission(permissionCodes, normalizedResourceKey, "delete", "remove"));
    }

    private async Task<HashSet<string>> ResolvePermissionCodesAsync(int tenantId, long userId, CancellationToken cancellationToken)
    {
        var userRoles = await userRoleApiClient.GetAllAsync(cancellationToken);
        var roleIds = userRoles
            .Where(userRole => userRole.TenantId == tenantId && userRole.UserId == userId)
            .Select(userRole => userRole.RoleId)
            .ToHashSet();

        if (roleIds.Count == 0)
        {
            return [];
        }

        var rolePermissions = await rolePermissionApiClient.GetAllAsync(cancellationToken);
        var permissionIds = rolePermissions
            .Where(rolePermission => rolePermission.TenantId == tenantId && roleIds.Contains(rolePermission.RoleId))
            .Select(rolePermission => rolePermission.PermissionId)
            .ToHashSet();

        if (permissionIds.Count == 0)
        {
            return [];
        }

        var permissions = await permissionApiClient.GetAllAsync(cancellationToken);
        return permissions
            .Where(permission => permissionIds.Contains(permission.Id))
            .Select(permission => NormalizeToken(permission.Code))
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    private static bool HasAnyPermission(IReadOnlySet<string> permissionCodes, string resourceKey, params string[] actions)
    {
        var normalizedActions = actions
            .Select(NormalizeToken)
            .Where(action => !string.IsNullOrWhiteSpace(action))
            .ToList();

        var resourceAliases = GetResourceAliases(resourceKey);

        return permissionCodes.Any(code =>
            normalizedActions.Any(action => resourceAliases.Any(alias => IsMatch(code, alias, action))));
    }

    private static bool IsMatch(string permissionCode, string resourceKey, string action)
    {
        var codeTokens = Tokenize(permissionCode).ToList();
        var resourceTokens = Tokenize(resourceKey).ToList();

        if (codeTokens.Count > 0 && resourceTokens.Count > 0)
        {
            var hasResource = resourceTokens.All(resourceToken =>
                codeTokens.Any(codeToken => IsEquivalentToken(codeToken, resourceToken)));

            if (hasResource)
            {
                if (codeTokens.Any(codeToken => IsManageAction(codeToken)))
                {
                    return true;
                }

                if (codeTokens.Any(codeToken => IsEquivalentToken(codeToken, action)))
                {
                    return true;
                }
            }
        }

        if (permissionCode == $"{resourceKey}_{action}" || permissionCode == $"{action}_{resourceKey}")
        {
            return true;
        }

        var compactCode = permissionCode.Replace("_", string.Empty, StringComparison.Ordinal);
        var compactResource = resourceKey.Replace("_", string.Empty, StringComparison.Ordinal);

        return compactCode == $"{compactResource}{action}" || compactCode == $"{action}{compactResource}";
    }

    private static IReadOnlyList<string> GetResourceAliases(string resourceKey)
    {
        return resourceKey switch
        {
            "tenant_setting" => ["tenant_setting", "settings", "setting", "tenantsettings"],
            "role_permission" => ["role_permission", "role_permissions", "rolepermission"],
            "pos_terminal" => ["pos_terminal", "terminal", "posterminal"],
            _ => [resourceKey]
        };
    }

    private static IEnumerable<string> Tokenize(string value)
        => NormalizeToken(value)
            .Split(['.', '_'], StringSplitOptions.RemoveEmptyEntries)
            .Where(token => !string.IsNullOrWhiteSpace(token));

    private static bool IsEquivalentToken(string left, string right)
    {
        if (left.Equals(right, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        var singularLeft = left.EndsWith('s') ? left[..^1] : left;
        var singularRight = right.EndsWith('s') ? right[..^1] : right;

        return singularLeft.Equals(singularRight, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsManageAction(string token)
        => token.Equals("manage", StringComparison.OrdinalIgnoreCase);

    private static string NormalizeToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return string.Empty;
        }

        return token.Trim().Replace(' ', '_').ToLower(CultureInfo.InvariantCulture);
    }
}

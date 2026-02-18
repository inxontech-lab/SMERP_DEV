using Domain.SaasDBModels;
using Microsoft.EntityFrameworkCore;

namespace SMERPAPIs.Services.SaasServices;

public class ModuleService(SmerpContext context) : IModuleService
{
    public async Task<List<PermittedModuleDto>> GetPermittedModulesAsync(int tenantId, long userId, CancellationToken cancellationToken = default)
    {
        var modules = await (
            from userRole in context.UserRoles
            join role in context.Roles on new { userRole.TenantId, userRole.RoleId } equals new { role.TenantId, RoleId = role.Id }
            join rolePermission in context.RolePermissions on new { userRole.TenantId, userRole.RoleId } equals new { rolePermission.TenantId, rolePermission.RoleId }
            join permission in context.Permissions on rolePermission.PermissionId equals permission.Id
            where userRole.TenantId == tenantId
                  && userRole.UserId == userId
                  && role.IsActive
                  && !string.IsNullOrWhiteSpace(permission.Module)
            select permission.Module!.Trim())
            .Distinct()
            .OrderBy(module => module)
            .ToListAsync(cancellationToken);

        return modules
            .Select(moduleName => new PermittedModuleDto(ToModuleKey(moduleName), moduleName))
            .DistinctBy(module => module.ModuleKey)
            .ToList();
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

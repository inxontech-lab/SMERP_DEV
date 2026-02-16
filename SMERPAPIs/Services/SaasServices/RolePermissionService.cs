using Domain.SaasDBModels;
using Domain.SaasReqDTO;
using Microsoft.EntityFrameworkCore;

namespace SMERPAPIs.Services.SaasServices;

public class RolePermissionService : IRolePermissionService
{
    private readonly SmerpContext _context;

    public RolePermissionService(SmerpContext context)
    {
        _context = context;
    }

    public async Task<List<RolePermission>> GetAllAsync()
    {
        return await _context.RolePermissions.AsNoTracking().ToListAsync();
    }

    public async Task<RolePermission?> GetByIdAsync(int tenantId, int roleId, int permissionId)
    {
        return await _context.RolePermissions.FindAsync(tenantId, roleId, permissionId);
    }

    public async Task<RolePermission> CreateAsync(RolePermissionRequest request)
    {
        var entity = new RolePermission
        {
            TenantId = request.TenantId,
            RoleId = request.RoleId,
            PermissionId = request.PermissionId
        };

        _context.RolePermissions.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> UpdateAsync(int tenantId, int roleId, int permissionId, RolePermissionRequest request)
    {
        var existingEntity = await _context.RolePermissions.FindAsync(tenantId, roleId, permissionId);
        if (existingEntity is null)
        {
            return false;
        }

        existingEntity.TenantId = request.TenantId;
        existingEntity.RoleId = request.RoleId;
        existingEntity.PermissionId = request.PermissionId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int tenantId, int roleId, int permissionId)
    {
        var existingEntity = await _context.RolePermissions.FindAsync(tenantId, roleId, permissionId);
        if (existingEntity is null)
        {
            return false;
        }

        _context.RolePermissions.Remove(existingEntity);
        await _context.SaveChangesAsync();
        return true;
    }
}

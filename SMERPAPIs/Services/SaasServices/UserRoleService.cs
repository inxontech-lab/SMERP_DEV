using Domain.SaasDBModels;
using Domain.SaasReqDTO;
using Microsoft.EntityFrameworkCore;

namespace SMERPAPIs.Services.SaasServices;

public class UserRoleService : IUserRoleService
{
    private readonly SmerpContext _context;

    public UserRoleService(SmerpContext context)
    {
        _context = context;
    }

    public async Task<List<UserRole>> GetAllAsync()
    {
        return await _context.UserRoles.AsNoTracking().ToListAsync();
    }

    public async Task<UserRole?> GetByIdAsync(int tenantId, long userId, int roleId)
    {
        return await _context.UserRoles.FindAsync(tenantId, userId, roleId);
    }

    public async Task<UserRole> CreateAsync(UserRoleRequest request)
    {
        var entity = new UserRole
        {
            TenantId = request.TenantId,
            UserId = request.UserId,
            RoleId = request.RoleId
        };

        _context.UserRoles.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> UpdateAsync(int tenantId, long userId, int roleId, UserRoleRequest request)
    {
        var existingEntity = await _context.UserRoles.FindAsync(tenantId, userId, roleId);
        if (existingEntity is null)
        {
            return false;
        }

        existingEntity.TenantId = request.TenantId;
        existingEntity.UserId = request.UserId;
        existingEntity.RoleId = request.RoleId;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int tenantId, long userId, int roleId)
    {
        var existingEntity = await _context.UserRoles.FindAsync(tenantId, userId, roleId);
        if (existingEntity is null)
        {
            return false;
        }

        _context.UserRoles.Remove(existingEntity);
        await _context.SaveChangesAsync();
        return true;
    }
}

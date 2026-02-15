using Domain.SaasDBModels;
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

    public async Task<UserRole> CreateAsync(UserRole entity)
    {
        _context.UserRoles.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> UpdateAsync(int tenantId, long userId, int roleId, UserRole entity)
    {
        var existingEntity = await _context.UserRoles.FindAsync(tenantId, userId, roleId);
        if (existingEntity is null)
        {
            return false;
        }

        _context.Entry(existingEntity).CurrentValues.SetValues(entity);
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

using Domain.SaasDBModels;
using Microsoft.EntityFrameworkCore;

namespace SMERPAPIs.Services.SaasServices;

public class UserBranchService : IUserBranchService
{
    private readonly SmerpContext _context;

    public UserBranchService(SmerpContext context)
    {
        _context = context;
    }

    public async Task<List<UserBranch>> GetAllAsync()
    {
        return await _context.UserBranches.AsNoTracking().ToListAsync();
    }

    public async Task<UserBranch?> GetByIdAsync(int tenantId, long userId, int branchId)
    {
        return await _context.UserBranches.FindAsync(tenantId, userId, branchId);
    }

    public async Task<UserBranch> CreateAsync(UserBranch entity)
    {
        _context.UserBranches.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> UpdateAsync(int tenantId, long userId, int branchId, UserBranch entity)
    {
        var existingEntity = await _context.UserBranches.FindAsync(tenantId, userId, branchId);
        if (existingEntity is null)
        {
            return false;
        }

        _context.Entry(existingEntity).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int tenantId, long userId, int branchId)
    {
        var existingEntity = await _context.UserBranches.FindAsync(tenantId, userId, branchId);
        if (existingEntity is null)
        {
            return false;
        }

        _context.UserBranches.Remove(existingEntity);
        await _context.SaveChangesAsync();
        return true;
    }
}

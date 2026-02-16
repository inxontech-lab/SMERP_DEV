using Domain.SaasDBModels;
using Domain.SaasReqDTO;
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

    public async Task<UserBranch> CreateAsync(UserBranchRequest request)
    {
        var entity = new UserBranch
        {
            TenantId = request.TenantId,
            UserId = request.UserId,
            BranchId = request.BranchId
        };

        _context.UserBranches.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<bool> UpdateAsync(int tenantId, long userId, int branchId, UserBranchRequest request)
    {
        var existingEntity = await _context.UserBranches.FindAsync(tenantId, userId, branchId);
        if (existingEntity is null)
        {
            return false;
        }

        existingEntity.TenantId = request.TenantId;
        existingEntity.UserId = request.UserId;
        existingEntity.BranchId = request.BranchId;

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

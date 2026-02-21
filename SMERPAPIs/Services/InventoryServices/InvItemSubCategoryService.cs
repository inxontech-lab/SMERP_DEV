using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Microsoft.EntityFrameworkCore;

namespace SMERPAPIs.Services.InventoryServices;

public class InvItemSubCategoryService(SmerpContext context) : IInvItemSubCategoryService
{
    public async Task<List<InvItemSubCategory>> GetAllAsync(int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var query = context.InvItemSubCategories.AsNoTracking().AsQueryable();
        if (viewerTenantId > 1) query = query.Where(x => x.TenantId == viewerTenantId);
        return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public async Task<InvItemSubCategory?> GetByIdAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var entity = await context.InvItemSubCategories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity is null || !TenantAccessGuard.CanAccess(viewerTenantId, entity.TenantId) ? null : entity;
    }

    public async Task<InvItemSubCategory> CreateAsync(CreateInvItemSubCategoryRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        TenantAccessGuard.EnsureAccess(viewerTenantId, request.TenantId, "inventory item sub-categories");
        var entity = new InvItemSubCategory
        {
            TenantId = request.TenantId,
            CategoryId = request.CategoryId,
            Code = request.Code,
            Name = request.Name,
            NameAr = request.NameAr,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = request.CreatedBy
        };
        context.InvItemSubCategories.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> UpdateAsync(int id, UpdateInvItemSubCategoryRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var entity = await context.InvItemSubCategories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null || !TenantAccessGuard.CanAccess(viewerTenantId, entity.TenantId)) return false;

        TenantAccessGuard.EnsureAccess(viewerTenantId, request.TenantId, "inventory item sub-categories");
        entity.TenantId = request.TenantId;
        entity.CategoryId = request.CategoryId;
        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.NameAr = request.NameAr;
        entity.IsActive = request.IsActive;
        entity.ModifiedAt = DateTime.UtcNow;
        entity.ModifiedBy = request.ModifiedBy;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var entity = await context.InvItemSubCategories.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null || !TenantAccessGuard.CanAccess(viewerTenantId, entity.TenantId)) return false;

        context.InvItemSubCategories.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

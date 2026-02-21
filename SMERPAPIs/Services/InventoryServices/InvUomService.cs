using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Microsoft.EntityFrameworkCore;

namespace SMERPAPIs.Services.InventoryServices;

public class InvUomService(SmerpContext context) : IInvUomService
{
    public async Task<List<InvUom>> GetAllAsync(int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var query = context.InvUoms.AsNoTracking().AsQueryable();
        if (viewerTenantId > 1)
        {
            query = query.Where(x => x.TenantId == viewerTenantId);
        }

        return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public async Task<InvUom?> GetByIdAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var entity = await context.InvUoms.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity is null || !TenantAccessGuard.CanAccess(viewerTenantId, entity.TenantId) ? null : entity;
    }

    public async Task<InvUom> CreateAsync(CreateInvUomRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        TenantAccessGuard.EnsureAccess(viewerTenantId, request.TenantId, "inventory UOMs");

        var entity = new InvUom
        {
            TenantId = request.TenantId,
            Code = request.Code,
            Name = request.Name,
            NameAr = request.NameAr,
            IsBase = request.IsBase,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = request.CreatedBy
        };

        context.InvUoms.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> UpdateAsync(int id, UpdateInvUomRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var entity = await context.InvUoms.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null || !TenantAccessGuard.CanAccess(viewerTenantId, entity.TenantId))
        {
            return false;
        }

        TenantAccessGuard.EnsureAccess(viewerTenantId, request.TenantId, "inventory UOMs");

        entity.TenantId = request.TenantId;
        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.NameAr = request.NameAr;
        entity.IsBase = request.IsBase;
        entity.IsActive = request.IsActive;
        entity.ModifiedAt = DateTime.UtcNow;
        entity.ModifiedBy = request.ModifiedBy;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var entity = await context.InvUoms.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null || !TenantAccessGuard.CanAccess(viewerTenantId, entity.TenantId))
        {
            return false;
        }

        context.InvUoms.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

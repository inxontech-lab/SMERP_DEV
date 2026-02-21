using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Microsoft.EntityFrameworkCore;

namespace SMERPAPIs.Services.InventoryServices;

public class InvItemUomconversionService(SmerpContext context) : IInvItemUomconversionService
{
    public async Task<List<InvItemUomconversion>> GetAllAsync(int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var query = context.InvItemUomconversions.AsNoTracking().AsQueryable();
        if (viewerTenantId > 1) query = query.Where(x => x.TenantId == viewerTenantId);
        return await query.OrderBy(x => x.Id).ToListAsync(cancellationToken);
    }

    public async Task<InvItemUomconversion?> GetByIdAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var entity = await context.InvItemUomconversions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity is null || !TenantAccessGuard.CanAccess(viewerTenantId, entity.TenantId) ? null : entity;
    }

    public async Task<InvItemUomconversion> CreateAsync(CreateInvItemUomconversionRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        TenantAccessGuard.EnsureAccess(viewerTenantId, request.TenantId, "inventory item UOM conversions");
        var entity = new InvItemUomconversion
        {
            TenantId = request.TenantId,
            ItemId = request.ItemId,
            FromUomid = request.FromUomid,
            ToUomid = request.ToUomid,
            Factor = request.Factor,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = request.CreatedBy
        };
        context.InvItemUomconversions.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> UpdateAsync(int id, UpdateInvItemUomconversionRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var entity = await context.InvItemUomconversions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null || !TenantAccessGuard.CanAccess(viewerTenantId, entity.TenantId)) return false;

        TenantAccessGuard.EnsureAccess(viewerTenantId, request.TenantId, "inventory item UOM conversions");
        entity.TenantId = request.TenantId;
        entity.ItemId = request.ItemId;
        entity.FromUomid = request.FromUomid;
        entity.ToUomid = request.ToUomid;
        entity.Factor = request.Factor;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var entity = await context.InvItemUomconversions.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null || !TenantAccessGuard.CanAccess(viewerTenantId, entity.TenantId)) return false;

        context.InvItemUomconversions.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

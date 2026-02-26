using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Microsoft.EntityFrameworkCore;

namespace SMERPAPIs.Services.InventoryServices;

public class InvWarehouseService(SmerpContext context) : IInvWarehouseService
{
    public async Task<List<InvWarehouse>> GetAllAsync(int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var query = context.InvWarehouses.AsNoTracking().AsQueryable();
        if (viewerTenantId > 1) query = query.Where(x => x.TenantId == viewerTenantId);
        return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public async Task<InvWarehouse?> GetByIdAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var entity = await context.InvWarehouses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity is null || !TenantAccessGuard.CanAccess(viewerTenantId, entity.TenantId) ? null : entity;
    }

    public async Task<InvWarehouse> CreateAsync(CreateInvWarehouseRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        TenantAccessGuard.EnsureAccess(viewerTenantId, request.TenantId, "inventory warehouses");
        var entity = new InvWarehouse
        {
            TenantId = request.TenantId,
            Code = request.Code,
            Name = request.Name,
            NameAr = request.NameAr,
            Address = request.Address,
            IsDefault = request.IsDefault,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = request.CreatedBy
        };
        context.InvWarehouses.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> UpdateAsync(int id, UpdateInvWarehouseRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var entity = await context.InvWarehouses.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null || !TenantAccessGuard.CanAccess(viewerTenantId, entity.TenantId)) return false;

        TenantAccessGuard.EnsureAccess(viewerTenantId, request.TenantId, "inventory warehouses");
        entity.TenantId = request.TenantId;
        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.NameAr = request.NameAr;
        entity.Address = request.Address;
        entity.IsDefault = request.IsDefault;
        entity.IsActive = request.IsActive;
        entity.ModifiedAt = DateTime.UtcNow;
        entity.ModifiedBy = request.ModifiedBy;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var entity = await context.InvWarehouses.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null || !TenantAccessGuard.CanAccess(viewerTenantId, entity.TenantId)) return false;

        context.InvWarehouses.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

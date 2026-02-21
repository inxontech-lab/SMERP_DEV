using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Microsoft.EntityFrameworkCore;

namespace SMERPAPIs.Services.InventoryServices;

public class InvItemService(SmerpContext context) : IInvItemService
{
    public async Task<List<InvItem>> GetAllAsync(int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var query = context.InvItems.AsNoTracking().AsQueryable();
        if (viewerTenantId > 1) query = query.Where(x => x.TenantId == viewerTenantId);
        return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public async Task<InvItem?> GetByIdAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var entity = await context.InvItems.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity is null || !TenantAccessGuard.CanAccess(viewerTenantId, entity.TenantId) ? null : entity;
    }

    public async Task<InvItem> CreateAsync(CreateInvItemRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        TenantAccessGuard.EnsureAccess(viewerTenantId, request.TenantId, "inventory items");
        var entity = new InvItem
        {
            TenantId = request.TenantId,
            Code = request.Code,
            Barcode = request.Barcode,
            Name = request.Name,
            NameAr = request.NameAr,
            CategoryId = request.CategoryId,
            SubCategoryId = request.SubCategoryId,
            BaseUomid = request.BaseUomid,
            PurchaseUomid = request.PurchaseUomid,
            SalesUomid = request.SalesUomid,
            Hscode = request.Hscode,
            CountryOfOrigin = request.CountryOfOrigin,
            Vatpercent = request.Vatpercent,
            StandardCost = request.StandardCost,
            SellingPrice = request.SellingPrice,
            TrackBatch = request.TrackBatch,
            TrackExpiry = request.TrackExpiry,
            MinStockLevel = request.MinStockLevel,
            MaxStockLevel = request.MaxStockLevel,
            ReorderLevel = request.ReorderLevel,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = request.CreatedBy
        };
        context.InvItems.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> UpdateAsync(int id, UpdateInvItemRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var entity = await context.InvItems.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null || !TenantAccessGuard.CanAccess(viewerTenantId, entity.TenantId)) return false;

        TenantAccessGuard.EnsureAccess(viewerTenantId, request.TenantId, "inventory items");
        entity.TenantId = request.TenantId;
        entity.Code = request.Code;
        entity.Barcode = request.Barcode;
        entity.Name = request.Name;
        entity.NameAr = request.NameAr;
        entity.CategoryId = request.CategoryId;
        entity.SubCategoryId = request.SubCategoryId;
        entity.BaseUomid = request.BaseUomid;
        entity.PurchaseUomid = request.PurchaseUomid;
        entity.SalesUomid = request.SalesUomid;
        entity.Hscode = request.Hscode;
        entity.CountryOfOrigin = request.CountryOfOrigin;
        entity.Vatpercent = request.Vatpercent;
        entity.StandardCost = request.StandardCost;
        entity.SellingPrice = request.SellingPrice;
        entity.TrackBatch = request.TrackBatch;
        entity.TrackExpiry = request.TrackExpiry;
        entity.MinStockLevel = request.MinStockLevel;
        entity.MaxStockLevel = request.MaxStockLevel;
        entity.ReorderLevel = request.ReorderLevel;
        entity.IsActive = request.IsActive;
        entity.ModifiedAt = DateTime.UtcNow;
        entity.ModifiedBy = request.ModifiedBy;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var entity = await context.InvItems.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null || !TenantAccessGuard.CanAccess(viewerTenantId, entity.TenantId)) return false;

        context.InvItems.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

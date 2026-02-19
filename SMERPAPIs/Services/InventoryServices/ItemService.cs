using Domain.SaasDBModels;
using Domain.SaasReqDTO;
using Microsoft.EntityFrameworkCore;

namespace SMERPAPIs.Services.InventoryServices;

public class ItemService(SmerpContext context) : IItemService
{
    public async Task<List<Product>> GetAllAsync(int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var query = context.Products.AsNoTracking().AsQueryable();

        if (viewerTenantId > 1)
        {
            query = query.Where(item => item.TenantId == viewerTenantId);
        }

        return await query.OrderBy(item => item.Name).ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(long id, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var item = await context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (item is null)
        {
            return null;
        }

        return CanAccessTenant(viewerTenantId, item.TenantId) ? item : null;
    }

    public async Task<Product> CreateAsync(ProductRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        ValidateTenantAccess(viewerTenantId, request.TenantId);

        var item = new Product
        {
            TenantId = request.TenantId,
            Sku = request.Sku,
            Barcode = request.Barcode,
            Name = request.Name,
            BaseUomId = request.BaseUomId,
            IsActive = request.IsActive,
            CreatedAt = request.CreatedAt == default ? DateTime.UtcNow : request.CreatedAt
        };

        context.Products.Add(item);
        await context.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task<bool> UpdateAsync(long id, ProductRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var item = await context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (item is null || !CanAccessTenant(viewerTenantId, item.TenantId))
        {
            return false;
        }

        ValidateTenantAccess(viewerTenantId, request.TenantId);

        item.TenantId = request.TenantId;
        item.Sku = request.Sku;
        item.Barcode = request.Barcode;
        item.Name = request.Name;
        item.BaseUomId = request.BaseUomId;
        item.IsActive = request.IsActive;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(long id, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var item = await context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (item is null || !CanAccessTenant(viewerTenantId, item.TenantId))
        {
            return false;
        }

        context.Products.Remove(item);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static bool CanAccessTenant(int viewerTenantId, int recordTenantId)
        => viewerTenantId <= 1 || viewerTenantId == recordTenantId;

    private static void ValidateTenantAccess(int viewerTenantId, int requestTenantId)
    {
        if (!CanAccessTenant(viewerTenantId, requestTenantId))
        {
            throw new InvalidOperationException("You can only manage items for your tenant.");
        }
    }
}

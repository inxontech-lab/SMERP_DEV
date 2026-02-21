using Domain.InvReqDTO;
using Domain.SaasDBModels;
using Microsoft.EntityFrameworkCore;

namespace SMERPAPIs.Services.InventoryServices;

public class InvSupplierService(SmerpContext context) : IInvSupplierService
{
    public async Task<List<InvSupplier>> GetAllAsync(int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var query = context.InvSuppliers.AsNoTracking().AsQueryable();
        if (viewerTenantId > 1) query = query.Where(x => x.TenantId == viewerTenantId);
        return await query.OrderBy(x => x.Name).ToListAsync(cancellationToken);
    }

    public async Task<InvSupplier?> GetByIdAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var entity = await context.InvSuppliers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity is null || !TenantAccessGuard.CanAccess(viewerTenantId, entity.TenantId) ? null : entity;
    }

    public async Task<InvSupplier> CreateAsync(CreateInvSupplierRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        TenantAccessGuard.EnsureAccess(viewerTenantId, request.TenantId, "inventory suppliers");
        var entity = new InvSupplier
        {
            TenantId = request.TenantId,
            Code = request.Code,
            Name = request.Name,
            NameAr = request.NameAr,
            ContactPerson = request.ContactPerson,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            AddressAr = request.AddressAr,
            CountryCode = request.CountryCode,
            VatregistrationNo = request.VatregistrationNo,
            Crno = request.Crno,
            PaymentTermsDays = request.PaymentTermsDays,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = request.CreatedBy
        };

        context.InvSuppliers.Add(entity);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task<bool> UpdateAsync(int id, UpdateInvSupplierRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var entity = await context.InvSuppliers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null || !TenantAccessGuard.CanAccess(viewerTenantId, entity.TenantId)) return false;

        TenantAccessGuard.EnsureAccess(viewerTenantId, request.TenantId, "inventory suppliers");
        entity.TenantId = request.TenantId;
        entity.Code = request.Code;
        entity.Name = request.Name;
        entity.NameAr = request.NameAr;
        entity.ContactPerson = request.ContactPerson;
        entity.Phone = request.Phone;
        entity.Email = request.Email;
        entity.Address = request.Address;
        entity.AddressAr = request.AddressAr;
        entity.CountryCode = request.CountryCode;
        entity.VatregistrationNo = request.VatregistrationNo;
        entity.Crno = request.Crno;
        entity.PaymentTermsDays = request.PaymentTermsDays;
        entity.IsActive = request.IsActive;
        entity.ModifiedAt = DateTime.UtcNow;
        entity.ModifiedBy = request.ModifiedBy;

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        var entity = await context.InvSuppliers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null || !TenantAccessGuard.CanAccess(viewerTenantId, entity.TenantId)) return false;

        context.InvSuppliers.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

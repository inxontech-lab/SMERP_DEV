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
        ValidateCreateRequest(request);
        TenantAccessGuard.EnsureAccess(viewerTenantId, request.TenantId, "inventory suppliers");
        await EnsureTenantExistsAsync(request.TenantId, cancellationToken);

        var entity = new InvSupplier
        {
            TenantId = request.TenantId,
            Code = request.Code.Trim(),
            Name = request.Name.Trim(),
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

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("Unable to save supplier. Please verify tenant and unique supplier values.");
        }

        return entity;
    }

    public async Task<bool> UpdateAsync(int id, UpdateInvSupplierRequest request, int viewerTenantId, CancellationToken cancellationToken = default)
    {
        ValidateUpdateRequest(request);
        var entity = await context.InvSuppliers.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null || !TenantAccessGuard.CanAccess(viewerTenantId, entity.TenantId)) return false;

        TenantAccessGuard.EnsureAccess(viewerTenantId, request.TenantId, "inventory suppliers");
        await EnsureTenantExistsAsync(request.TenantId, cancellationToken);

        entity.TenantId = request.TenantId;
        entity.Code = request.Code.Trim();
        entity.Name = request.Name.Trim();
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

        try
        {
            await context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            throw new InvalidOperationException("Unable to update supplier. Please verify tenant and unique supplier values.");
        }

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

    private async Task EnsureTenantExistsAsync(int tenantId, CancellationToken cancellationToken)
    {
        var exists = await context.Tenants.AsNoTracking().AnyAsync(t => t.Id == tenantId, cancellationToken);
        if (!exists)
        {
            throw new InvalidOperationException("Selected tenant is invalid.");
        }
    }

    private static void ValidateCreateRequest(CreateInvSupplierRequest request)
    {
        if (request.TenantId <= 0)
        {
            throw new InvalidOperationException("Tenant is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            throw new InvalidOperationException("Code is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidOperationException("Name is required.");
        }
    }

    private static void ValidateUpdateRequest(UpdateInvSupplierRequest request)
    {
        if (request.Id <= 0)
        {
            throw new InvalidOperationException("Supplier id is invalid.");
        }

        if (request.TenantId <= 0)
        {
            throw new InvalidOperationException("Tenant is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Code))
        {
            throw new InvalidOperationException("Code is required.");
        }

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidOperationException("Name is required.");
        }
    }
}

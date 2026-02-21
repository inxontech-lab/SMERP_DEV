using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class InvGrnheader
{
    public long Id { get; set; }

    public int TenantId { get; set; }

    public int BranchId { get; set; }

    public int WarehouseId { get; set; }

    public string Grnno { get; set; } = null!;

    public DateOnly Grndate { get; set; }

    public int SupplierId { get; set; }

    public string? SupplierInvoiceNo { get; set; }

    public DateOnly? SupplierInvoiceDate { get; set; }

    public string? CurrencyCode { get; set; }

    public decimal ExchangeRate { get; set; }

    public string Status { get; set; } = null!;

    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual ICollection<InvGrnline> InvGrnlines { get; set; } = new List<InvGrnline>();

    public virtual InvSupplier Supplier { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual InvWarehouse Warehouse { get; set; } = null!;
}

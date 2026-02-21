using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class InvStockTxn
{
    public long Id { get; set; }

    public int TenantId { get; set; }

    public int BranchId { get; set; }

    public int WarehouseId { get; set; }

    public int ItemId { get; set; }

    public DateTime TxnDate { get; set; }

    public string TxnType { get; set; } = null!;

    public string? RefType { get; set; }

    public long? RefId { get; set; }

    public string? RefNo { get; set; }

    public string? BatchNo { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public decimal QtyIn { get; set; }

    public decimal QtyOut { get; set; }

    public decimal UnitCost { get; set; }

    public decimal? Amount { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual InvItem Item { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual InvWarehouse Warehouse { get; set; } = null!;
}

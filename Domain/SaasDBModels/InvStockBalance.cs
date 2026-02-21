using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class InvStockBalance
{
    public long Id { get; set; }

    public int TenantId { get; set; }

    public int BranchId { get; set; }

    public int WarehouseId { get; set; }

    public int ItemId { get; set; }

    public string? BatchNo { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public decimal QtyOnHand { get; set; }

    public decimal QtyReserved { get; set; }

    public decimal AvgCost { get; set; }

    public DateTime? LastTxnAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual InvItem Item { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual InvWarehouse Warehouse { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class ProductBranchStock
{
    public long Id { get; set; }

    public int TenantId { get; set; }

    public long ProductId { get; set; }

    public int BranchId { get; set; }

    public decimal OnHandQty { get; set; }

    public decimal ReservedQty { get; set; }

    public decimal ReorderLevel { get; set; }

    public decimal MaxStockLevel { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}

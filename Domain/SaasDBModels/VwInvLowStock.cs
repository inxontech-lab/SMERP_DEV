using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class VwInvLowStock
{
    public int TenantId { get; set; }

    public int BranchId { get; set; }

    public int WarehouseId { get; set; }

    public int ItemId { get; set; }

    public string ItemCode { get; set; } = null!;

    public string ItemName { get; set; } = null!;

    public string? ItemNameAr { get; set; }

    public decimal? AvailableQty { get; set; }

    public decimal? ReorderLevel { get; set; }
}

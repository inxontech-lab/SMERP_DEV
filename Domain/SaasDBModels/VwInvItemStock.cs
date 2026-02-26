using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class VwInvItemStock
{
    public int TenantId { get; set; }

    public int BranchId { get; set; }

    public string BranchCode { get; set; } = null!;

    public string BranchName { get; set; } = null!;

    public int WarehouseId { get; set; }

    public string WarehouseCode { get; set; } = null!;

    public string WarehouseName { get; set; } = null!;

    public string? WarehouseNameAr { get; set; }

    public int ItemId { get; set; }

    public string ItemCode { get; set; } = null!;

    public string ItemName { get; set; } = null!;

    public string? ItemNameAr { get; set; }

    public string? BatchNo { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public decimal QtyOnHand { get; set; }

    public decimal QtyReserved { get; set; }

    public decimal? QtyAvailable { get; set; }

    public decimal AvgCost { get; set; }

    public decimal? StockValue { get; set; }
}

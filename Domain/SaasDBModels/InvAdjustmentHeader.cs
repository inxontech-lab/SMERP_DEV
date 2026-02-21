using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class InvAdjustmentHeader
{
    public long Id { get; set; }

    public int TenantId { get; set; }

    public int BranchId { get; set; }

    public int WarehouseId { get; set; }

    public string AdjustNo { get; set; } = null!;

    public DateOnly AdjustDate { get; set; }

    public string? Reason { get; set; }

    public string? ReasonAr { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? PostedAt { get; set; }

    public string? PostedBy { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual ICollection<InvAdjustmentLine> InvAdjustmentLines { get; set; } = new List<InvAdjustmentLine>();

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual InvWarehouse Warehouse { get; set; } = null!;
}

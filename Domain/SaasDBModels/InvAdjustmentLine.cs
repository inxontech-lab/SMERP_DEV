using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class InvAdjustmentLine
{
    public long Id { get; set; }

    public int TenantId { get; set; }

    public long AdjustmentHeaderId { get; set; }

    public int LineNo { get; set; }

    public int ItemId { get; set; }

    public int Uomid { get; set; }

    public decimal QtyDelta { get; set; }

    public decimal UnitCost { get; set; }

    public string? BatchNo { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public string? Reason { get; set; }

    public virtual InvAdjustmentHeader AdjustmentHeader { get; set; } = null!;

    public virtual InvItem Item { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual InvUom Uom { get; set; } = null!;
}

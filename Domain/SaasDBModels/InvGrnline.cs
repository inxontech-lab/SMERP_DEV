using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class InvGrnline
{
    public long Id { get; set; }

    public int TenantId { get; set; }

    public long GrnheaderId { get; set; }

    public int LineNo { get; set; }

    public int ItemId { get; set; }

    public int Uomid { get; set; }

    public decimal Qty { get; set; }

    public decimal UnitCost { get; set; }

    public decimal Vatpercent { get; set; }

    public string? BatchNo { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public string? Remarks { get; set; }

    public virtual InvGrnheader Grnheader { get; set; } = null!;

    public virtual InvItem Item { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual InvUom Uom { get; set; } = null!;
}

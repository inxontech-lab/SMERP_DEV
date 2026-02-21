using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class InvItemUomconversion
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public int ItemId { get; set; }

    public int FromUomid { get; set; }

    public int ToUomid { get; set; }

    public decimal Factor { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public virtual InvUom FromUom { get; set; } = null!;

    public virtual InvItem Item { get; set; } = null!;

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual InvUom ToUom { get; set; } = null!;
}

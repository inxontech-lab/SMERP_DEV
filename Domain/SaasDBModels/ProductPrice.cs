using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class ProductPrice
{
    public long Id { get; set; }

    public int TenantId { get; set; }

    public long ProductId { get; set; }

    public int UomId { get; set; }

    public int? BranchId { get; set; }

    public byte PriceType { get; set; }

    public decimal Price { get; set; }

    public decimal? MinQty { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; }

    public virtual Branch? Branch { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Uom Uom { get; set; } = null!;
}

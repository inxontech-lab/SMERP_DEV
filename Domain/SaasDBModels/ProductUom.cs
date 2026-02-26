using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class ProductUom
{
    public long Id { get; set; }

    public int TenantId { get; set; }

    public long ProductId { get; set; }

    public int UomId { get; set; }

    public decimal FactorToBase { get; set; }

    public decimal QtyStep { get; set; }

    public byte MaxDecimals { get; set; }

    public bool IsSellable { get; set; }

    public bool IsPurchasable { get; set; }

    public bool IsDefaultSell { get; set; }

    public bool IsDefaultBuy { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Uom Uom { get; set; } = null!;
}

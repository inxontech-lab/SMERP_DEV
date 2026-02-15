using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class ProductPrice
{
    public long Id { get; set; }

    public int TenantId { get; set; }

    public long ProductId { get; set; }

    public int TaxCodeId { get; set; }

    public decimal SellPrice { get; set; }

    public bool IsVatInclusive { get; set; }

    public int DefaultSellUomId { get; set; }

    public virtual Uom DefaultSellUom { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual TaxCode TaxCode { get; set; } = null!;
}

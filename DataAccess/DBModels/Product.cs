using System;
using System.Collections.Generic;

namespace DataAccess.DBModels;

public partial class Product
{
    public long Id { get; set; }

    public int TenantId { get; set; }

    public string Sku { get; set; } = null!;

    public string? Barcode { get; set; }

    public string Name { get; set; } = null!;

    public int BaseUomId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Uom BaseUom { get; set; } = null!;

    public virtual ICollection<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();

    public virtual ICollection<ProductUom> ProductUoms { get; set; } = new List<ProductUom>();
}

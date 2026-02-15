using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class Uom
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public byte UomType { get; set; }

    public virtual ICollection<ProductPrice> ProductPrices { get; set; } = new List<ProductPrice>();

    public virtual ICollection<ProductUom> ProductUoms { get; set; } = new List<ProductUom>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

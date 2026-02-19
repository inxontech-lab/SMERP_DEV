using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class TaxCode
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal Rate { get; set; }

    public bool IsZeroRated { get; set; }

    public bool IsExempt { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

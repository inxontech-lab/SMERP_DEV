using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class ProductCategoryMap
{
    public long Id { get; set; }

    public int TenantId { get; set; }

    public long ProductId { get; set; }

    public int CategoryId { get; set; }

    public bool IsPrimary { get; set; }

    public virtual ProductCategory Category { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}

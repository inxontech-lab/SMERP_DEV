using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class ProductCategory
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public int? ParentCategoryId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? NameArabic { get; set; }

    public bool IsActive { get; set; }

    public virtual ProductCategory? ParentCategory { get; set; }

    public virtual ICollection<ProductCategory> InverseParentCategory { get; set; } = new List<ProductCategory>();

    public virtual ICollection<ProductCategoryMap> ProductCategoryMaps { get; set; } = new List<ProductCategoryMap>();
}

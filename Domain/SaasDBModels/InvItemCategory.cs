using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class InvItemCategory
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? NameAr { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual ICollection<InvItemSubCategory> InvItemSubCategories { get; set; } = new List<InvItemSubCategory>();

    public virtual ICollection<InvItem> InvItems { get; set; } = new List<InvItem>();

    public virtual Tenant Tenant { get; set; } = null!;
}

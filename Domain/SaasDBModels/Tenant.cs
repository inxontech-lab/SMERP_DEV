using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class Tenant
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? LegalName { get; set; }

    public string? VatNo { get; set; }

    public string? CrNo { get; set; }

    public string CountryCode { get; set; } = null!;

    public string TimeZone { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public virtual TenantSetting? TenantSetting { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

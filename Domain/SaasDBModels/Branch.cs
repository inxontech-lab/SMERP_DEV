using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class Branch
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<PosTerminal> PosTerminals { get; set; } = new List<PosTerminal>();

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual ICollection<UserBranch> UserBranches { get; set; } = new List<UserBranch>();
}

using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class TenantModule
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public string ModuleId { get; set; } = null!;

    public bool Active { get; set; }

    public DateTime CreatedAt { get; set; }
}

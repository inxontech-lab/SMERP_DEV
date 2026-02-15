using System;
using System.Collections.Generic;

namespace DataAccess.DBModels;

public partial class PosTerminal
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public int BranchId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool IsKiosk { get; set; }

    public bool IsActive { get; set; }

    public virtual Branch Branch { get; set; } = null!;
}

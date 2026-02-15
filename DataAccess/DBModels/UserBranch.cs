using System;
using System.Collections.Generic;

namespace DataAccess.DBModels;

public partial class UserBranch
{
    public int TenantId { get; set; }

    public long UserId { get; set; }

    public int BranchId { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}

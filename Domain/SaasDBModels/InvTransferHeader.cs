using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class InvTransferHeader
{
    public long Id { get; set; }

    public int TenantId { get; set; }

    public string TransferNo { get; set; } = null!;

    public DateOnly TransferDate { get; set; }

    public int FromBranchId { get; set; }

    public int FromWarehouseId { get; set; }

    public int ToBranchId { get; set; }

    public int ToWarehouseId { get; set; }

    public string Status { get; set; } = null!;

    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ReceivedAt { get; set; }

    public string? ReceivedBy { get; set; }

    public virtual Branch FromBranch { get; set; } = null!;

    public virtual InvWarehouse FromWarehouse { get; set; } = null!;

    public virtual ICollection<InvTransferLine> InvTransferLines { get; set; } = new List<InvTransferLine>();

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual Branch ToBranch { get; set; } = null!;

    public virtual InvWarehouse ToWarehouse { get; set; } = null!;
}

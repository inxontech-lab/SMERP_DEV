using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class InvWarehouse
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public int BranchId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? NameAr { get; set; }

    public string? Address { get; set; }

    public bool IsDefault { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual Branch Branch { get; set; } = null!;

    public virtual ICollection<InvAdjustmentHeader> InvAdjustmentHeaders { get; set; } = new List<InvAdjustmentHeader>();

    public virtual ICollection<InvGrnheader> InvGrnheaders { get; set; } = new List<InvGrnheader>();

    public virtual ICollection<InvStockBalance> InvStockBalances { get; set; } = new List<InvStockBalance>();

    public virtual ICollection<InvStockTxn> InvStockTxns { get; set; } = new List<InvStockTxn>();

    public virtual ICollection<InvTransferHeader> InvTransferHeaderFromWarehouses { get; set; } = new List<InvTransferHeader>();

    public virtual ICollection<InvTransferHeader> InvTransferHeaderToWarehouses { get; set; } = new List<InvTransferHeader>();

    public virtual Tenant Tenant { get; set; } = null!;
}

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

    public virtual ICollection<InvAdjustmentHeader> InvAdjustmentHeaders { get; set; } = new List<InvAdjustmentHeader>();

    public virtual ICollection<InvGrnheader> InvGrnheaders { get; set; } = new List<InvGrnheader>();

    public virtual ICollection<InvStockBalance> InvStockBalances { get; set; } = new List<InvStockBalance>();

    public virtual ICollection<InvStockTxn> InvStockTxns { get; set; } = new List<InvStockTxn>();

    public virtual ICollection<InvTransferHeader> InvTransferHeaderFromBranches { get; set; } = new List<InvTransferHeader>();

    public virtual ICollection<InvTransferHeader> InvTransferHeaderToBranches { get; set; } = new List<InvTransferHeader>();


    public virtual ICollection<PosTerminal> PosTerminals { get; set; } = new List<PosTerminal>();

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual ICollection<UserBranch> UserBranches { get; set; } = new List<UserBranch>();
}

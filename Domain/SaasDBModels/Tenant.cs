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

    public virtual ICollection<InvAdjustmentHeader> InvAdjustmentHeaders { get; set; } = new List<InvAdjustmentHeader>();

    public virtual ICollection<InvAdjustmentLine> InvAdjustmentLines { get; set; } = new List<InvAdjustmentLine>();

    public virtual ICollection<InvGrnheader> InvGrnheaders { get; set; } = new List<InvGrnheader>();

    public virtual ICollection<InvGrnline> InvGrnlines { get; set; } = new List<InvGrnline>();

    public virtual ICollection<InvItemCategory> InvItemCategories { get; set; } = new List<InvItemCategory>();

    public virtual ICollection<InvItemSubCategory> InvItemSubCategories { get; set; } = new List<InvItemSubCategory>();

    public virtual ICollection<InvItemUomconversion> InvItemUomconversions { get; set; } = new List<InvItemUomconversion>();

    public virtual ICollection<InvItem> InvItems { get; set; } = new List<InvItem>();

    public virtual ICollection<InvStockBalance> InvStockBalances { get; set; } = new List<InvStockBalance>();

    public virtual ICollection<InvStockTxn> InvStockTxns { get; set; } = new List<InvStockTxn>();

    public virtual ICollection<InvSupplier> InvSuppliers { get; set; } = new List<InvSupplier>();

    public virtual ICollection<InvTransferHeader> InvTransferHeaders { get; set; } = new List<InvTransferHeader>();

    public virtual ICollection<InvTransferLine> InvTransferLines { get; set; } = new List<InvTransferLine>();

    public virtual ICollection<InvUom> InvUoms { get; set; } = new List<InvUom>();

    public virtual ICollection<InvWarehouse> InvWarehouses { get; set; } = new List<InvWarehouse>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public virtual TenantSetting? TenantSetting { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

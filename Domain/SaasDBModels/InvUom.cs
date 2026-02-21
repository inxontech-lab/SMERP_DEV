using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class InvUom
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? NameAr { get; set; }

    public bool IsBase { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual ICollection<InvAdjustmentLine> InvAdjustmentLines { get; set; } = new List<InvAdjustmentLine>();

    public virtual ICollection<InvGrnline> InvGrnlines { get; set; } = new List<InvGrnline>();

    public virtual ICollection<InvItem> InvItemBaseUoms { get; set; } = new List<InvItem>();

    public virtual ICollection<InvItem> InvItemPurchaseUoms { get; set; } = new List<InvItem>();

    public virtual ICollection<InvItem> InvItemSalesUoms { get; set; } = new List<InvItem>();

    public virtual ICollection<InvItemUomconversion> InvItemUomconversionFromUoms { get; set; } = new List<InvItemUomconversion>();

    public virtual ICollection<InvItemUomconversion> InvItemUomconversionToUoms { get; set; } = new List<InvItemUomconversion>();

    public virtual ICollection<InvTransferLine> InvTransferLines { get; set; } = new List<InvTransferLine>();

    public virtual Tenant Tenant { get; set; } = null!;
}

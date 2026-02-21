using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class InvItem
{
    public int Id { get; set; }

    public int TenantId { get; set; }

    public string Code { get; set; } = null!;

    public string? Barcode { get; set; }

    public string Name { get; set; } = null!;

    public string? NameAr { get; set; }

    public int CategoryId { get; set; }

    public int? SubCategoryId { get; set; }

    public int BaseUomid { get; set; }

    public int? PurchaseUomid { get; set; }

    public int? SalesUomid { get; set; }

    public string? Hscode { get; set; }

    public string? CountryOfOrigin { get; set; }

    public decimal Vatpercent { get; set; }

    public decimal StandardCost { get; set; }

    public decimal SellingPrice { get; set; }

    public bool TrackBatch { get; set; }

    public bool TrackExpiry { get; set; }

    public decimal? MinStockLevel { get; set; }

    public decimal? MaxStockLevel { get; set; }

    public decimal? ReorderLevel { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual InvUom BaseUom { get; set; } = null!;

    public virtual InvItemCategory Category { get; set; } = null!;

    public virtual ICollection<InvAdjustmentLine> InvAdjustmentLines { get; set; } = new List<InvAdjustmentLine>();

    public virtual ICollection<InvGrnline> InvGrnlines { get; set; } = new List<InvGrnline>();

    public virtual ICollection<InvItemUomconversion> InvItemUomconversions { get; set; } = new List<InvItemUomconversion>();

    public virtual ICollection<InvStockBalance> InvStockBalances { get; set; } = new List<InvStockBalance>();

    public virtual ICollection<InvStockTxn> InvStockTxns { get; set; } = new List<InvStockTxn>();

    public virtual ICollection<InvTransferLine> InvTransferLines { get; set; } = new List<InvTransferLine>();

    public virtual InvUom? PurchaseUom { get; set; }

    public virtual InvUom? SalesUom { get; set; }

    public virtual InvItemSubCategory? SubCategory { get; set; }

    public virtual Tenant Tenant { get; set; } = null!;
}

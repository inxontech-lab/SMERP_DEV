namespace Domain.InvReqDTO;

public class CreateInvItemRequest
{
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
    public string? CreatedBy { get; set; }
}

public class UpdateInvItemRequest
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
    public string? ModifiedBy { get; set; }
}

public class DeleteInvItemRequest
{
    public int Id { get; set; }
    public int TenantId { get; set; }
}

public class GetInvItemRequest
{
    public int Id { get; set; }
    public int TenantId { get; set; }
}

public class ListInvItemRequest
{
    public int TenantId { get; set; }
    public int? CategoryId { get; set; }
    public int? SubCategoryId { get; set; }
    public bool? IsActive { get; set; }
}

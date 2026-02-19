namespace Domain.SaasReqDTO;

public class ProductRequest
{
    public int TenantId { get; set; }

    public string Sku { get; set; } = null!;

    public string? Barcode { get; set; }

    public string Name { get; set; } = null!;

    public string? NameArabic { get; set; }

    public string? ShortName { get; set; }

    public string? Description { get; set; }

    public string? DescriptionArabic { get; set; }

    public int BaseUomId { get; set; }

    public int? TaxCodeId { get; set; }

    public bool IsVatApplicable { get; set; }

    public byte VatPricingMethod { get; set; }

    public bool IsBatchTracked { get; set; }

    public bool IsSerialTracked { get; set; }

    public bool IsStockItem { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}

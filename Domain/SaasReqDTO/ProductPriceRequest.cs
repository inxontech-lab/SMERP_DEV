namespace Domain.SaasReqDTO;

public class ProductPriceRequest
{
    public int TenantId { get; set; }

    public long ProductId { get; set; }

    public int UomId { get; set; }

    public int? BranchId { get; set; }

    public byte PriceType { get; set; }

    public decimal Price { get; set; }

    public decimal? MinQty { get; set; }

    public DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public bool IsActive { get; set; }
}

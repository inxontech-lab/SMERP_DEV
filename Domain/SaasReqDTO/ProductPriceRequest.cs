namespace Domain.SaasReqDTO;

public class ProductPriceRequest
{
    public int TenantId { get; set; }

    public long ProductId { get; set; }

    public int TaxCodeId { get; set; }

    public decimal SellPrice { get; set; }

    public bool IsVatInclusive { get; set; }

    public int DefaultSellUomId { get; set; }
}

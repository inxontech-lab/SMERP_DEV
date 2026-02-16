namespace Domain.SaasReqDTO;

public class ProductUomRequest
{
    public int TenantId { get; set; }

    public long ProductId { get; set; }

    public int UomId { get; set; }

    public decimal FactorToBase { get; set; }

    public decimal QtyStep { get; set; }

    public byte MaxDecimals { get; set; }

    public bool IsSellable { get; set; }

    public bool IsPurchasable { get; set; }

    public bool IsDefaultSell { get; set; }

    public bool IsDefaultBuy { get; set; }
}

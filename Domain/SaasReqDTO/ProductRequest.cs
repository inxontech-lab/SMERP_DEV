namespace Domain.SaasReqDTO;

public class ProductRequest
{
    public int TenantId { get; set; }

    public string Sku { get; set; } = null!;

    public string? Barcode { get; set; }

    public string Name { get; set; } = null!;

    public int BaseUomId { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}

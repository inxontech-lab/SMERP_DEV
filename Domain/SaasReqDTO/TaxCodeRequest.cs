namespace Domain.SaasReqDTO;

public class TaxCodeRequest
{
    public int TenantId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public decimal Rate { get; set; }

    public bool IsZeroRated { get; set; }

    public bool IsExempt { get; set; }

    public bool IsActive { get; set; }
}

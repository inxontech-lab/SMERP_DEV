namespace Domain.SaasReqDTO;

public class TenantRequest
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? LegalName { get; set; }

    public string? VatNo { get; set; }

    public string? CrNo { get; set; }

    public string CountryCode { get; set; } = null!;

    public string TimeZone { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }
}

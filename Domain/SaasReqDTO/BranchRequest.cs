namespace Domain.SaasReqDTO;

public class BranchRequest
{
    public int TenantId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public bool IsActive { get; set; }
}

namespace Domain.SaasReqDTO;

public class PosTerminalRequest
{
    public int TenantId { get; set; }

    public int BranchId { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool IsKiosk { get; set; }

    public bool IsActive { get; set; }
}

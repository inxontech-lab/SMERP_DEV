namespace Domain.SaasReqDTO;

public class RoleRequest
{
    public int TenantId { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }
}

namespace Domain.SaasReqDTO;

public class RolePermissionRequest
{
    public int TenantId { get; set; }

    public int RoleId { get; set; }

    public int PermissionId { get; set; }
}

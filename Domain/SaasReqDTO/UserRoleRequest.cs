namespace Domain.SaasReqDTO;

public class UserRoleRequest
{
    public int TenantId { get; set; }

    public long UserId { get; set; }

    public int RoleId { get; set; }
}

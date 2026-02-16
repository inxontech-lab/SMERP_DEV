namespace Domain.SaasReqDTO;

public class UserBranchRequest
{
    public int TenantId { get; set; }

    public long UserId { get; set; }

    public int BranchId { get; set; }
}

namespace Domain.SaasReqDTO;

public class UserWithRoleResponse
{
    public long UserId { get; set; }

    public int TenantId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Mobile { get; set; }

    public bool IsActive { get; set; }

    public int? RoleId { get; set; }

    public string? RoleName { get; set; }
}

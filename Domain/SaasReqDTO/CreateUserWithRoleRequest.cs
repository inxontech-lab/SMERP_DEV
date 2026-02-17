namespace Domain.SaasReqDTO;

public class CreateUserWithRoleRequest
{
    public int TenantId { get; set; }

    public int RoleId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? Mobile { get; set; }

    public bool IsActive { get; set; } = true;
}

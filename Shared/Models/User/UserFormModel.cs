using Domain.SaasReqDTO;

namespace SMERPWeb.Models.User;

public class UserFormModel
{
    public int TenantId { get; set; }
    public int RoleId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Password { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public bool IsActive { get; set; } = true;

    public CreateUserWithRoleRequest ToCreateRequest() => new()
    {
        TenantId = TenantId,
        RoleId = RoleId,
        Username = Username,
        DisplayName = DisplayName,
        Password = Password,
        Email = Email,
        Mobile = Mobile,
        IsActive = IsActive
    };

    public UpdateUserWithRoleRequest ToUpdateRequest() => new()
    {
        TenantId = TenantId,
        RoleId = RoleId,
        Username = Username,
        DisplayName = DisplayName,
        Password = string.IsNullOrWhiteSpace(Password) ? null : Password,
        Email = Email,
        Mobile = Mobile,
        IsActive = IsActive
    };

    public static UserFormModel FromResponse(UserWithRoleResponse user) => new()
    {
        TenantId = user.TenantId,
        RoleId = user.RoleId ?? 0,
        Username = user.Username,
        DisplayName = user.DisplayName,
        Email = user.Email,
        Mobile = user.Mobile,
        IsActive = user.IsActive,
        Password = string.Empty
    };
}

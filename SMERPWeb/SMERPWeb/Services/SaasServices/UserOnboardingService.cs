using System.Security.Cryptography;
using System.Text;
using Domain.SaasDBModels;
using Domain.SaasReqDTO;

namespace SMERPWeb.Services.SaasServices;

public interface IUserOnboardingService
{
    Task<List<Tenant>> GetTenantsAsync(CancellationToken cancellationToken = default);
    Task<List<Permission>> GetPermissionsAsync(CancellationToken cancellationToken = default);
    Task<long> CreateUserWithPermissionAsync(CreateUserWithPermissionRequest request, CancellationToken cancellationToken = default);
}

public class UserOnboardingService(
    ITenantApiClient tenantApiClient,
    IPermissionApiClient permissionApiClient,
    IRolePermissionApiClient rolePermissionApiClient,
    IUserRequestApiClient userRequestApiClient,
    IUserRoleRequestApiClient userRoleRequestApiClient) : IUserOnboardingService
{
    public Task<List<Tenant>> GetTenantsAsync(CancellationToken cancellationToken = default)
        => tenantApiClient.GetAllAsync(cancellationToken);

    public Task<List<Permission>> GetPermissionsAsync(CancellationToken cancellationToken = default)
        => permissionApiClient.GetAllAsync(cancellationToken);

    public async Task<long> CreateUserWithPermissionAsync(CreateUserWithPermissionRequest request, CancellationToken cancellationToken = default)
    {
        var matchingRolePermission = (await rolePermissionApiClient.GetAllAsync(cancellationToken))
            .FirstOrDefault(item => item.TenantId == request.TenantId && item.PermissionId == request.PermissionId);

        if (matchingRolePermission is null)
        {
            throw new InvalidOperationException("No role is mapped with the selected tenant and permission.");
        }

        var (passwordHash, passwordSalt) = HashPassword(request.Password);

        var createdUser = await userRequestApiClient.CreateAsync(new UserRequest
        {
            TenantId = request.TenantId,
            Username = request.Username,
            DisplayName = request.DisplayName,
            Email = request.Email,
            Mobile = request.Mobile,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = null
        }, cancellationToken);

        await userRoleRequestApiClient.CreateAsync(new UserRoleRequest
        {
            TenantId = request.TenantId,
            UserId = createdUser.Id,
            RoleId = matchingRolePermission.RoleId
        }, cancellationToken);

        return createdUser.Id;
    }

    private static (byte[] PasswordHash, byte[] PasswordSalt) HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var payload = new byte[salt.Length + passwordBytes.Length];

        Buffer.BlockCopy(salt, 0, payload, 0, salt.Length);
        Buffer.BlockCopy(passwordBytes, 0, payload, salt.Length, passwordBytes.Length);

        var hash = SHA256.HashData(payload);
        return (hash, salt);
    }
}

public class CreateUserWithPermissionRequest
{
    public int TenantId { get; set; }
    public int PermissionId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public bool IsActive { get; set; } = true;
}

using System.Security.Cryptography;
using System.Text;
using Domain.SaasDBModels;
using Domain.SaasReqDTO;
using Microsoft.EntityFrameworkCore;

namespace SMERPAPIs.Services.SaasServices;

public class UserOnboardingService(SmerpContext context) : IUserOnboardingService
{
    public async Task<long> CreateUserWithRoleAsync(CreateUserWithRoleRequest request)
    {
        var role = await context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == request.RoleId && item.TenantId == request.TenantId);

        if (role is null)
        {
            throw new InvalidOperationException("Selected role is not available for the selected tenant.");
        }

        var (passwordHash, passwordSalt) = HashPassword(request.Password);

        var user = new User
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
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        var userRole = new UserRole
        {
            TenantId = request.TenantId,
            UserId = user.Id,
            RoleId = request.RoleId
        };

        context.UserRoles.Add(userRole);
        await context.SaveChangesAsync();

        return user.Id;
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

using System.Security.Cryptography;
using System.Text;
using Domain.SaasDBModels;
using Domain.SaasReqDTO;
using Microsoft.EntityFrameworkCore;

namespace SMERPAPIs.Services.SaasServices;

public class UserOnboardingService(SmerpContext context) : IUserOnboardingService
{
    public async Task<List<UserWithRoleResponse>> GetUsersWithRolesAsync()
    {
        var query =
            from user in context.Users.AsNoTracking()
            join userRole in context.UserRoles.AsNoTracking() on user.Id equals userRole.UserId into userRoleGroup
            from userRole in userRoleGroup.DefaultIfEmpty()
            join role in context.Roles.AsNoTracking() on userRole.RoleId equals role.Id into roleGroup
            from role in roleGroup.DefaultIfEmpty()
            select new UserWithRoleResponse
            {
                UserId = user.Id,
                TenantId = user.TenantId,
                Username = user.Username,
                DisplayName = user.DisplayName,
                Email = user.Email,
                Mobile = user.Mobile,
                IsActive = user.IsActive,
                RoleId = userRole != null ? userRole.RoleId : null,
                RoleName = role != null ? role.Name : null
            };

        return await query.ToListAsync();
    }

    public async Task<long> CreateUserWithRoleAsync(CreateUserWithRoleRequest request)
    {
        var role = await EnsureValidRoleAsync(request.TenantId, request.RoleId);
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

        context.UserRoles.Add(new UserRole
        {
            TenantId = request.TenantId,
            UserId = user.Id,
            RoleId = request.RoleId
        });

        await context.SaveChangesAsync();
        return user.Id;
    }

    public async Task<bool> UpdateUserWithRoleAsync(long userId, UpdateUserWithRoleRequest request)
    {
        var user = await context.Users.FirstOrDefaultAsync(item => item.Id == userId);
        if (user is null)
        {
            return false;
        }

        var role = await EnsureValidRoleAsync(request.TenantId, request.RoleId);
        if (role is null)
        {
            throw new InvalidOperationException("Selected role is not available for the selected tenant.");
        }

        user.TenantId = request.TenantId;
        user.Username = request.Username;
        user.DisplayName = request.DisplayName;
        user.Email = request.Email;
        user.Mobile = request.Mobile;
        user.IsActive = request.IsActive;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            var (passwordHash, passwordSalt) = HashPassword(request.Password);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
        }

        var existingRoles = await context.UserRoles.Where(item => item.UserId == userId).ToListAsync();
        if (existingRoles.Count != 0)
        {
            context.UserRoles.RemoveRange(existingRoles);
        }

        context.UserRoles.Add(new UserRole
        {
            TenantId = request.TenantId,
            UserId = userId,
            RoleId = request.RoleId
        });

        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUserWithRoleAsync(long userId)
    {
        var user = await context.Users.FirstOrDefaultAsync(item => item.Id == userId);
        if (user is null)
        {
            return false;
        }

        var existingRoles = await context.UserRoles.Where(item => item.UserId == userId).ToListAsync();
        if (existingRoles.Count != 0)
        {
            context.UserRoles.RemoveRange(existingRoles);
        }

        context.Users.Remove(user);
        await context.SaveChangesAsync();
        return true;
    }

    private async Task<Role?> EnsureValidRoleAsync(int tenantId, int roleId)
    {
        return await context.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == roleId && item.TenantId == tenantId);
    }

    private static (byte[] PasswordHash, byte[] PasswordSalt) HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(32);
        var passwordBytes = Encoding.UTF8.GetBytes(password);
        var hash = Rfc2898DeriveBytes.Pbkdf2(passwordBytes, salt, 100000, HashAlgorithmName.SHA512, 64);
        return (hash, salt);
    }
}

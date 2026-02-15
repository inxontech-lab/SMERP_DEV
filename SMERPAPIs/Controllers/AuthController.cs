using System.Security.Cryptography;
using System.Text;
using Domain.SaasDBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SMERPAPIs.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly SmerpContext _context;

    public AuthController(SmerpContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var normalizedUsername = request.Username.Trim();

        var user = await _context.Users
            .AsTracking()
            .FirstOrDefaultAsync(x =>
                x.TenantId == request.TenantId &&
                x.Username == normalizedUsername &&
                x.IsActive);

        if (user is null || !VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            return Unauthorized(new { message = "Invalid username or password." });
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return Ok(new LoginResponse(
            user.Id,
            user.TenantId,
            user.Username,
            user.DisplayName,
            user.Email,
            user.Mobile,
            user.LastLoginAt));
    }

    private static bool VerifyPassword(string password, byte[] storedHash, byte[] storedSalt)
    {
        if (storedHash.Length == 0 || storedSalt.Length == 0)
        {
            return false;
        }

        var passwordBytes = Encoding.UTF8.GetBytes(password);

        using var hmac = new HMACSHA512(storedSalt);
        var hmacHash = hmac.ComputeHash(passwordBytes);
        if (FixedTimeEquals(storedHash, hmacHash))
        {
            return true;
        }

        using var sha = SHA512.Create();
        var salted = new byte[storedSalt.Length + passwordBytes.Length];
        Buffer.BlockCopy(storedSalt, 0, salted, 0, storedSalt.Length);
        Buffer.BlockCopy(passwordBytes, 0, salted, storedSalt.Length, passwordBytes.Length);
        var shaHash = sha.ComputeHash(salted);
        if (FixedTimeEquals(storedHash, shaHash))
        {
            return true;
        }

        var pbkdf2Hash = Rfc2898DeriveBytes.Pbkdf2(passwordBytes, storedSalt, 100000, HashAlgorithmName.SHA512, storedHash.Length);
        return FixedTimeEquals(storedHash, pbkdf2Hash);
    }

    private static bool FixedTimeEquals(byte[] left, byte[] right)
    {
        return left.Length == right.Length && CryptographicOperations.FixedTimeEquals(left, right);
    }
}

public record LoginRequest(int TenantId, string Username, string Password);

public record LoginResponse(
    long Id,
    int TenantId,
    string Username,
    string DisplayName,
    string? Email,
    string? Mobile,
    DateTime? LastLoginAt);

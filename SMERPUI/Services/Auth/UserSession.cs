namespace SMERPUI.Services.Auth;

public sealed record UserSession(
    long UserId,
    int TenantId,
    string Username,
    string DisplayName,
    string? Email,
    string? Mobile,
    DateTime? LastLoginAt);

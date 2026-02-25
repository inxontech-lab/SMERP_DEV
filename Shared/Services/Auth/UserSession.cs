namespace Shared.Services.Auth;

public sealed record UserSession(
    long UserId,
    int TenantId,
    string TenantName,
    string Username,
    string DisplayName,
    string? Email,
    string? Mobile,
    DateTime? LastLoginAt,
    string? SelectedModule = null);

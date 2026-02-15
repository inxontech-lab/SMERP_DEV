using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class RefreshToken
{
    public long Id { get; set; }

    public int TenantId { get; set; }

    public long UserId { get; set; }

    public byte[] TokenHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? CreatedIp { get; set; }

    public string? UserAgent { get; set; }

    public virtual User User { get; set; } = null!;
}

using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class User
{
    public long Id { get; set; }

    public int TenantId { get; set; }

    public string Username { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public string? Email { get; set; }

    public string? Mobile { get; set; }

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual Tenant Tenant { get; set; } = null!;

    public virtual ICollection<UserBranch> UserBranches { get; set; } = new List<UserBranch>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

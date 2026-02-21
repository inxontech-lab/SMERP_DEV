using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class AuditLog1
{
    public long Id { get; set; }

    public int? TenantId { get; set; }

    public int? BranchId { get; set; }

    public string TableName { get; set; } = null!;

    public string RecordId { get; set; } = null!;

    public string ActionType { get; set; } = null!;

    public string? OldData { get; set; }

    public string? NewData { get; set; }

    public DateTime ChangedAt { get; set; }

    public string? ChangedBy { get; set; }

    public string? AppName { get; set; }

    public string? HostName { get; set; }
}

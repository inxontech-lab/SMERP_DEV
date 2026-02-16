namespace Domain.SaasReqDTO;

public class AuditLogRequest
{
    public int TenantId { get; set; }

    public long? UserId { get; set; }

    public string Action { get; set; } = null!;

    public string? Entity { get; set; }

    public string? EntityId { get; set; }

    public string? Details { get; set; }

    public DateTime CreatedAt { get; set; }
}

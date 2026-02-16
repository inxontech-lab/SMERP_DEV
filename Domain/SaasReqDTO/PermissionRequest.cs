namespace Domain.SaasReqDTO;

public class PermissionRequest
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Module { get; set; }
}

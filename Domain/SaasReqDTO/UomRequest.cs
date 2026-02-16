namespace Domain.SaasReqDTO;

public class UomRequest
{
    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public byte UomType { get; set; }
}

namespace Domain.InvReqDTO;

public class CreateInvUomRequest
{
    public int TenantId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? NameAr { get; set; }
    public bool IsBase { get; set; }
    public bool IsActive { get; set; }
    public string? CreatedBy { get; set; }
}

public class UpdateInvUomRequest
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? NameAr { get; set; }
    public bool IsBase { get; set; }
    public bool IsActive { get; set; }
    public string? ModifiedBy { get; set; }
}

public class DeleteInvUomRequest
{
    public int Id { get; set; }
    public int TenantId { get; set; }
}

public class GetInvUomRequest
{
    public int Id { get; set; }
    public int TenantId { get; set; }
}

public class ListInvUomRequest
{
    public int TenantId { get; set; }
    public bool? IsActive { get; set; }
}

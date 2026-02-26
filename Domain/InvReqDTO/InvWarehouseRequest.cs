namespace Domain.InvReqDTO;

public class CreateInvWarehouseRequest
{
    public int TenantId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? NameAr { get; set; }
    public string? Address { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public string? CreatedBy { get; set; }
}

public class UpdateInvWarehouseRequest
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? NameAr { get; set; }
    public string? Address { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public string? ModifiedBy { get; set; }
}

public class DeleteInvWarehouseRequest
{
    public int Id { get; set; }
    public int TenantId { get; set; }
}

public class GetInvWarehouseRequest
{
    public int Id { get; set; }
    public int TenantId { get; set; }
}

public class ListInvWarehouseRequest
{
    public int TenantId { get; set; }
    public bool? IsActive { get; set; }
}

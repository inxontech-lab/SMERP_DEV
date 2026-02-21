namespace Domain.InvReqDTO;

public class CreateInvSupplierRequest
{
    public int TenantId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? NameAr { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? AddressAr { get; set; }
    public string? CountryCode { get; set; }
    public string? VatregistrationNo { get; set; }
    public string? Crno { get; set; }
    public int? PaymentTermsDays { get; set; }
    public bool IsActive { get; set; }
    public string? CreatedBy { get; set; }
}

public class UpdateInvSupplierRequest
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? NameAr { get; set; }
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? AddressAr { get; set; }
    public string? CountryCode { get; set; }
    public string? VatregistrationNo { get; set; }
    public string? Crno { get; set; }
    public int? PaymentTermsDays { get; set; }
    public bool IsActive { get; set; }
    public string? ModifiedBy { get; set; }
}

public class DeleteInvSupplierRequest
{
    public int Id { get; set; }
    public int TenantId { get; set; }
}

public class GetInvSupplierRequest
{
    public int Id { get; set; }
    public int TenantId { get; set; }
}

public class ListInvSupplierRequest
{
    public int TenantId { get; set; }
    public bool? IsActive { get; set; }
}

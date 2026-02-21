using System;
using System.Collections.Generic;

namespace Domain.SaasDBModels;

public partial class InvSupplier
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

    public DateTime CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? ModifiedBy { get; set; }

    public virtual ICollection<InvGrnheader> InvGrnheaders { get; set; } = new List<InvGrnheader>();

    public virtual Tenant Tenant { get; set; } = null!;
}

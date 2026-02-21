namespace Domain.InvReqDTO;

public class CreateInvItemCategoryRequest
{
    public int TenantId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? NameAr { get; set; }
    public bool IsActive { get; set; }
    public string? CreatedBy { get; set; }
}

public class UpdateInvItemCategoryRequest
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? NameAr { get; set; }
    public bool IsActive { get; set; }
    public string? ModifiedBy { get; set; }
}

public class DeleteInvItemCategoryRequest
{
    public int Id { get; set; }
    public int TenantId { get; set; }
}

public class GetInvItemCategoryRequest
{
    public int Id { get; set; }
    public int TenantId { get; set; }
}

public class ListInvItemCategoryRequest
{
    public int TenantId { get; set; }
    public bool? IsActive { get; set; }
}

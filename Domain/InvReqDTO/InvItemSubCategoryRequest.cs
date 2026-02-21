namespace Domain.InvReqDTO;

public class CreateInvItemSubCategoryRequest
{
    public int TenantId { get; set; }
    public int CategoryId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? NameAr { get; set; }
    public bool IsActive { get; set; }
    public string? CreatedBy { get; set; }
}

public class UpdateInvItemSubCategoryRequest
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int CategoryId { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? NameAr { get; set; }
    public bool IsActive { get; set; }
    public string? ModifiedBy { get; set; }
}

public class DeleteInvItemSubCategoryRequest
{
    public int Id { get; set; }
    public int TenantId { get; set; }
}

public class GetInvItemSubCategoryRequest
{
    public int Id { get; set; }
    public int TenantId { get; set; }
}

public class ListInvItemSubCategoryRequest
{
    public int TenantId { get; set; }
    public int? CategoryId { get; set; }
    public bool? IsActive { get; set; }
}

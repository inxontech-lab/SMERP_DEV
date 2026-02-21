namespace Domain.InvReqDTO;

public class CreateInvItemUomconversionRequest
{
    public int TenantId { get; set; }
    public int ItemId { get; set; }
    public int FromUomid { get; set; }
    public int ToUomid { get; set; }
    public decimal Factor { get; set; }
    public string? CreatedBy { get; set; }
}

public class UpdateInvItemUomconversionRequest
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int ItemId { get; set; }
    public int FromUomid { get; set; }
    public int ToUomid { get; set; }
    public decimal Factor { get; set; }
}

public class DeleteInvItemUomconversionRequest
{
    public int Id { get; set; }
    public int TenantId { get; set; }
}

public class GetInvItemUomconversionRequest
{
    public int Id { get; set; }
    public int TenantId { get; set; }
}

public class ListInvItemUomconversionRequest
{
    public int TenantId { get; set; }
    public int? ItemId { get; set; }
}

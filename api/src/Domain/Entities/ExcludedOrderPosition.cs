namespace Domain.Entities;

public class ExcludedOrderPosition : BaseAuditableEntity
{
    public string OrderId { get; set; }
    public string PartNumber { get; set; }
}

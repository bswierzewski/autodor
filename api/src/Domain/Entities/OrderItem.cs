namespace Domain.Entities;

public class OrderItem
{
    public bool IsExcluded { get; set; }
    public string PartNumber { get; set; }
    public string PartName { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
}

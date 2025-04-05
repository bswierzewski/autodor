namespace Application.Orders.Queries
{
    public class OrderItemDto
    {
        public string PartNumber { get; set; }
        public string PartName { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

using Domain.Entities;

namespace Application.Orders.Queries
{
    public class OrderDto
    {
        public bool IsExcluded { get; set; }
        public DateTime Date { get; set; }
        public string Id { get; set; }
        public string Number { get; set; }
        public string Person { get; set; }
        public string CustomerNumber { get; set; }
        public IEnumerable<OrderItem> Items { get; set; }

        public int ItemsCount => Items?.Count() ?? 0;
        public decimal TotalPrice => Math.Round((Items?.Sum(x => x.TotalPrice) ?? 0) * 1.23M, 2);
    }
}

using MediatR;
using Microsoft.Extensions.Logging;

namespace Autodor.Modules.Orders.Application.Queries.GetOrdersByContractorAndPeriod;

public class GetOrdersByContractorAndPeriodQueryHandler : IRequestHandler<GetOrdersByContractorAndPeriodQuery, IEnumerable<GetOrdersByContractorAndPeriodDto>>
{
    private readonly ILogger<GetOrdersByContractorAndPeriodQueryHandler> _logger;

    public GetOrdersByContractorAndPeriodQueryHandler(ILogger<GetOrdersByContractorAndPeriodQueryHandler> logger)
    {
        _logger = logger;
    }

    public async Task<IEnumerable<GetOrdersByContractorAndPeriodDto>> Handle(GetOrdersByContractorAndPeriodQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting orders for contractor {ContractorId} from {DateFrom} to {DateTo}", 
            request.ContractorId, request.DateFrom, request.DateTo);

        // Symulowane dane zamówień - w rzeczywistej implementacji byłoby pobieranie z bazy danych
        var mockOrders = GenerateMockOrdersForContractor(request.ContractorId, request.DateFrom, request.DateTo);

        _logger.LogInformation("Found {OrderCount} orders for contractor {ContractorId} in specified period", 
            mockOrders.Count(), request.ContractorId);

        await Task.Delay(50, cancellationToken); // Symulacja opóźnienia bazy danych

        return mockOrders;
    }

    private static IEnumerable<GetOrdersByContractorAndPeriodDto> GenerateMockOrdersForContractor(Guid contractorId, DateTime dateFrom, DateTime dateTo)
    {
        var random = new Random(contractorId.GetHashCode());
        var orderCount = random.Next(3, 8); // 3-7 zamówień w okresie
        var orders = new List<GetOrdersByContractorAndPeriodDto>();

        for (int i = 0; i < orderCount; i++)
        {
            var orderDate = dateFrom.AddDays(random.Next(0, (dateTo - dateFrom).Days + 1));
            var orderNumber = $"ORD-{orderDate:yyyyMMdd}-{random.Next(1000, 9999)}";
            
            var itemCount = random.Next(1, 5); // 1-4 pozycje na zamówienie
            var items = new List<GetOrdersByContractorAndPeriodItemDto>();

            var availablePartNumbers = new[]
            {
                "PAR001", "PAR002", "PAR003", "PAR004", "PAR005",
                "PAR006", "PAR007", "PAR008", "PAR009", "PAR010"
            };

            for (int j = 0; j < itemCount; j++)
            {
                var partNumber = availablePartNumbers[random.Next(availablePartNumbers.Length)];
                var quantity = random.Next(1, 10);
                var unitPrice = Math.Round((decimal)(random.NextDouble() * 200 + 10), 2);
                var totalPrice = quantity * unitPrice;

                items.Add(new GetOrdersByContractorAndPeriodItemDto(partNumber, quantity, unitPrice, totalPrice));
            }

            orders.Add(new GetOrdersByContractorAndPeriodDto(orderNumber, orderDate, contractorId, items));
        }

        return orders.OrderBy(o => o.OrderDate);
    }
}
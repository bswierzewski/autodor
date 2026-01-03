using Application.Common.Interfaces;

namespace Application.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery() : IRequest<OrderDto>
{
    public string OrderId { get; init; }
}

public class GetOrderByIdQueryHandler(
    IApplicationDbContext context,
    IDistributorsSalesService distributorsSalesService,
    IProductsService productsService) : IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var orderTask = distributorsSalesService.GetOrderAsync(request.OrderId);
        var productsTask = productsService.GetProductsAsync();
        var excludedPositionsTask = context.ExcludedOrderPositions
            .Where(x => x.OrderId == request.OrderId)
            .Select(x => x.PartNumber)
            .ToListAsync(cancellationToken);

        await Task.WhenAll(orderTask, productsTask, excludedPositionsTask);

        var order = await orderTask;
        var products = await productsTask;
        var excludedPositions = await excludedPositionsTask;
        var excludedPositionSet = excludedPositions.ToHashSet();

        foreach (var item in order.Items)
        {
            item.IsExcluded = excludedPositionSet.Contains(item.PartNumber);

            // Enrich with product names
            var existsName = products.ContainsKey(item?.PartNumber ?? "");
            item.PartName = existsName
                ? $"{products[item.PartNumber].Name} ({item.PartNumber})"
                : item.PartNumber;
        }

        return new OrderDto
        {
            IsExcluded = false,
            Date = order.Date,
            Id = order.Id,
            Number = order.Number,
            Person = order.Person,
            CustomerNumber = order.CustomerNumber,
            Items = order.Items
        };
    }
}

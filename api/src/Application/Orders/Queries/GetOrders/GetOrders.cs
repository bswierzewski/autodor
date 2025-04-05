using Application.Common.Extensions;
using Application.Common.Interfaces;
using AutoMapper;
using Domain.Entities;

namespace Application.Orders.Queries;

public record GetOrdersQuery() : IRequest<IEnumerable<OrderDto>>
{
    public DateTime DateFrom { get; init; }
    public DateTime DateTo { get; init; }
}

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, IEnumerable<OrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IDistributorsSalesService _distributorsSalesService;

    public GetOrdersQueryHandler(IApplicationDbContext context,
        IMapper mapper,
        IDistributorsSalesService distributorsSalesService)
    {
        _context = context;
        _mapper = mapper;
        _distributorsSalesService = distributorsSalesService;
    }

    public async Task<IEnumerable<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = new List<Order>();

        foreach (var date in DateTimeExtensions.EachDay(request.DateFrom, request.DateTo))
            orders.AddRange(await _distributorsSalesService.GetOrdersAsync(date));

        // Fetch all orders within the date range concurrently
        var allDates = DateTimeExtensions.EachDay(request.DateFrom, request.DateTo);
        var allOrdersTasks = allDates.Select(_distributorsSalesService.GetOrdersAsync);
        var ordersList = await Task.WhenAll(allOrdersTasks);
        orders = ordersList.SelectMany(o => o).ToList();

        var ordersToReturn = _mapper.Map<IEnumerable<OrderDto>>(orders).ToList();

        var exludedOrders = _context.ExcludedOrders.Select(x => x.OrderId).ToHashSet();

        ordersToReturn.ForEach(order =>
        {
            order.IsExcluded = exludedOrders.Contains(order.Id);
        });

        return ordersToReturn;
    }
}
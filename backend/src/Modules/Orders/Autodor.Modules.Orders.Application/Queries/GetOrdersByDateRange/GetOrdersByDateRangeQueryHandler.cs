using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Domain.Entities;
using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrdersByDateRange;

/// <summary>
/// Handles queries for retrieving orders within a specified date range.
/// </summary>
public sealed class GetOrdersByDateRangeQueryHandler : IRequestHandler<GetOrdersByDateRangeQuery, IEnumerable<Order>>
{
    private readonly IOrdersRepository _ordersRepository;

    /// <summary>
    /// Initializes a new instance of the GetOrdersByDateRangeQueryHandler.
    /// </summary>
    /// <param name="ordersRepository">The repository for accessing order data.</param>
    public GetOrdersByDateRangeQueryHandler(IOrdersRepository ordersRepository)
    {
        _ordersRepository = ordersRepository;
    }

    /// <summary>
    /// Processes the query to retrieve orders within the specified date range.
    /// </summary>
    /// <param name="request">The query containing the date range criteria.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A collection of orders within the specified date range.</returns>
    public async Task<IEnumerable<Order>> Handle(GetOrdersByDateRangeQuery request, CancellationToken cancellationToken)
    {
        var orders = await _ordersRepository.GetOrdersByDateRangeAsync(request.DateFrom, request.DateTo);
        return orders;
    }
}
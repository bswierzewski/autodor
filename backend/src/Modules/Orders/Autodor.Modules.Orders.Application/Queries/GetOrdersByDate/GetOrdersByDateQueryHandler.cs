using Autodor.Modules.Orders.Application.Abstractions;
using Autodor.Modules.Orders.Domain.Entities;
using MediatR;

namespace Autodor.Modules.Orders.Application.Queries.GetOrdersByDate;

/// <summary>
/// Handles queries for retrieving orders by a specific date.
/// </summary>
public sealed class GetOrdersByDateQueryHandler : IRequestHandler<GetOrdersByDateQuery, IEnumerable<Order>>
{
    private readonly IOrdersRepository _ordersRepository;

    /// <summary>
    /// Initializes a new instance of the GetOrdersByDateQueryHandler.
    /// </summary>
    /// <param name="ordersRepository">The repository for accessing order data.</param>
    public GetOrdersByDateQueryHandler(IOrdersRepository ordersRepository)
    {
        _ordersRepository = ordersRepository;
    }

    /// <summary>
    /// Processes the query to retrieve orders for a specific date.
    /// </summary>
    /// <param name="request">The query containing the date criteria.</param>
    /// <param name="cancellationToken">A token to observe for cancellation requests.</param>
    /// <returns>A collection of orders for the specified date.</returns>
    public async Task<IEnumerable<Order>> Handle(GetOrdersByDateQuery request, CancellationToken cancellationToken)
    {
        var orders = await _ordersRepository.GetOrdersByDateAsync(request.Date);
        return orders;
    }
}
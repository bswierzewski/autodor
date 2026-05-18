namespace Autodor.Modules.Orders.Contracts.Queries;

/// <summary>
/// Query to get orders by specific dates
/// </summary>
public record GetOrdersByDatesQuery(IEnumerable<DateTime> Dates);
namespace Autodot.Modules.Orders.Contracts.Queries;

/// <summary>
/// Query to get orders by date range
/// </summary>
public record GetOrdersByDateRangeQuery(DateTime DateFrom, DateTime DateTo);

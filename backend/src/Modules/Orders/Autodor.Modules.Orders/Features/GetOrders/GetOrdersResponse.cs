namespace Autodor.Modules.Orders.Features.GetOrders;

/// <summary>
/// Response DTO for GetOrders endpoint
/// </summary>
public record GetOrdersResponse(
    List<OrderSummaryResponse> Orders
);

/// <summary>
/// Response DTO for Order Summary
/// </summary>
public record OrderSummaryResponse(
    string Id,
    string? Number,
    DateTime Date,
    string? Person,
    string? CustomerNumber,
    int ItemsCount,
    decimal TotalAmount
);

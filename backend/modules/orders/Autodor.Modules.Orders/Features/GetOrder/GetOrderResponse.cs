namespace Autodor.Modules.Orders.Features.GetOrder;

/// <summary>
/// Response DTO for GetOrder endpoint
/// </summary>
public record GetOrderResponse(
    string Id,
    string? Number,
    DateTime Date,
    string? Person,
    string? CustomerNumber,
    List<OrderItemResponse> Items,
    bool IsExcluded
);

/// <summary>
/// Response DTO for Order Item
/// </summary>
public record OrderItemResponse(
    string ProductDisplayName,
    int Quantity,
    decimal Price,
    bool IsExcluded
);

namespace Autodor.Modules.Orders.Features.PrintOrders;

public record PrintOrdersCommand(
    Guid[] OrderIds
);

using Autodor.Modules.Orders.Application.Commands.ExcludeOrder;
using Autodor.Modules.Orders.Application.Queries.GetOrdersByDate;
using Autodor.Modules.Orders.Application.Queries.GetOrdersByDateRange;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Autodor.Web.Endpoints;

/// <summary>
/// Provides HTTP endpoints for order management operations within the Autodor automotive parts trading system.
/// This class implements the REST API layer for business operations related to order processing, filtering,
/// and exclusion management. All endpoints follow the CQRS pattern using MediatR for command and query handling.
/// </summary>
/// <remarks>
/// The Orders module handles the core business logic for processing automotive parts orders,
/// including integration with external suppliers and order lifecycle management.
/// These endpoints expose the necessary operations for front-end applications and external integrations.
/// </remarks>
public static class OrdersEndpoints
{
    /// <summary>
    /// Configures and maps all HTTP endpoints for order management operations.
    /// This method establishes the routing structure for the Orders API, grouping all order-related
    /// endpoints under the "/api/orders" base path with appropriate OpenAPI documentation.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder used to configure API routes</param>
    /// <returns>The configured endpoint route builder for method chaining</returns>
    /// <remarks>
    /// All endpoints are configured with OpenAPI support for automatic API documentation generation.
    /// The "Orders" tag groups these endpoints in the Swagger UI for better organization.
    /// Each endpoint follows REST conventions and returns standardized HTTP status codes.
    /// </remarks>
    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Create a route group for all order endpoints with consistent base path and tagging
        // This approach ensures API consistency and simplifies routing management
        var group = endpoints.MapGroup("/api/orders")
            .WithTags("Orders");

        // POST /api/orders/exclude - Business operation to exclude problematic orders from processing
        // Returns 200 OK with success indicator - used when orders cannot be fulfilled or have issues
        group.MapPost("/exclude", ExcludeOrder)
            .WithName("ExcludeOrder")
            .WithOpenApi();

        // GET /api/orders/by-date - Query operation to retrieve orders for a specific business day
        // Critical for daily operations and order processing workflows
        group.MapGet("/by-date", GetOrdersByDate)
            .WithName("GetOrdersByDate")
            .WithOpenApi();

        // GET /api/orders/by-date-range - Query operation for historical order analysis and reporting
        // Supports business intelligence and period-based order tracking
        group.MapGet("/by-date-range", GetOrdersByDateRange)
            .WithName("GetOrdersByDateRange")
            .WithOpenApi();

        return endpoints;
    }

    /// <summary>
    /// Handles the business operation of excluding an order from the normal processing workflow.
    /// This endpoint is used when orders cannot be fulfilled due to supplier issues, quality problems,
    /// or other business constraints that prevent normal order completion.
    /// </summary>
    /// <param name="command">The command containing order exclusion details and business justification</param>
    /// <param name="mediator">MediatR instance for executing the command through the application layer</param>
    /// <returns>HTTP 200 OK with success indicator when exclusion is processed successfully</returns>
    /// <remarks>
    /// Order exclusion is a critical business operation that affects inventory management and customer communication.
    /// The operation is idempotent - excluding an already excluded order will not cause errors.
    /// This endpoint supports audit logging and business process compliance requirements.
    /// </remarks>
    private static async Task<IResult> ExcludeOrder(
        [FromBody] ExcludeOrderCommand command,
        IMediator mediator)
    {
        // Execute the exclusion command through the application layer
        // This ensures all business rules and domain validations are applied
        var result = await mediator.Send(command);
        
        // Return 200 OK with explicit success indicator for client-side processing
        // The boolean result indicates whether the exclusion was successfully recorded
        return Results.Ok(new { Success = result });
    }

    /// <summary>
    /// Retrieves all orders for a specific business date, supporting daily operational workflows.
    /// This endpoint is essential for daily order processing, fulfillment planning, and operational reporting.
    /// The date parameter is typically used to query orders created or scheduled for processing on that day.
    /// </summary>
    /// <param name="date">The specific date for which to retrieve orders (typically in YYYY-MM-DD format)</param>
    /// <param name="mediator">MediatR instance for executing the query through the application layer</param>
    /// <returns>HTTP 200 OK with a collection of orders matching the specified date criteria</returns>
    /// <remarks>
    /// This endpoint is crucial for daily operations and is frequently called by dashboard applications.
    /// The date filtering supports business processes like daily order fulfillment, shipping planning,
    /// and operational reporting. Results include all relevant order details for processing workflows.
    /// </remarks>
    private static async Task<IResult> GetOrdersByDate(
        DateTime date,
        IMediator mediator)
    {
        // Create query with the specified date for business date filtering
        // This supports daily operational workflows and reporting requirements
        var query = new GetOrdersByDateQuery(date);
        var result = await mediator.Send(query);
        
        // Return 200 OK with order collection - standard pattern for successful data retrieval
        // Empty collections are valid responses when no orders exist for the specified date
        return Results.Ok(result);
    }

    /// <summary>
    /// Retrieves orders within a specified date range, enabling historical analysis and period-based reporting.
    /// This endpoint supports business intelligence operations, financial reporting, and performance analysis
    /// by allowing queries across multiple days, weeks, or months of order data.
    /// </summary>
    /// <param name="dateFrom">The start date of the range (inclusive) for order filtering</param>
    /// <param name="dateTo">The end date of the range (inclusive) for order filtering</param>
    /// <param name="mediator">MediatR instance for executing the query through the application layer</param>
    /// <returns>HTTP 200 OK with a collection of orders within the specified date range</returns>
    /// <remarks>
    /// This endpoint is designed for reporting and analytics use cases where period-based data is required.
    /// Common scenarios include monthly sales reports, quarterly analysis, and trend identification.
    /// The date range is inclusive on both ends to provide intuitive business date filtering.
    /// Large date ranges may return substantial data sets - consider pagination for production use.
    /// </remarks>
    private static async Task<IResult> GetOrdersByDateRange(
        DateTime dateFrom,
        DateTime dateTo,
        IMediator mediator)
    {
        // Create query with date range parameters for period-based filtering
        // Supports business reporting and analytics requirements across multiple time periods
        var query = new GetOrdersByDateRangeQuery(dateFrom, dateTo);
        var result = await mediator.Send(query);
        
        // Return 200 OK with order collection - consistent with other query endpoints
        // Results may be large for extended date ranges, consider implementing pagination
        return Results.Ok(result);
    }
}
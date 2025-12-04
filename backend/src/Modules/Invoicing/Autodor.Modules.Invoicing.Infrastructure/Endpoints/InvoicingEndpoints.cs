using Autodor.Modules.Invoicing.Application.Commands.CreateBulkInvoices;
using Autodor.Modules.Invoicing.Application.Commands.CreateInvoice;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Infrastructure.Models;

namespace Autodor.Modules.Invoicing.Infrastructure.Endpoints;

/// <summary>
/// Provides HTTP endpoints for invoice management operations within the Autodor automotive parts trading system.
/// This class implements REST API endpoints for creating and managing invoices, supporting both individual
/// and bulk invoice generation workflows essential for business operations and regulatory compliance.
/// </summary>
/// <remarks>
/// The Invoicing module handles critical financial operations including invoice generation, tax calculations,
/// and regulatory compliance for Polish and European Union requirements. These endpoints support automated
/// invoicing workflows, bulk operations for efficiency, and integration with accounting systems.
/// All operations ensure proper audit trails and compliance with tax regulations.
/// </remarks>
public static class InvoicingEndpoints
{
    /// <summary>
    /// Configures and maps all HTTP endpoints for invoice management operations.
    /// This method establishes the routing structure for invoice-related API operations,
    /// supporting both individual and bulk invoice generation workflows.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder used to configure API routes</param>
    /// <returns>The configured endpoint route builder for method chaining</returns>
    /// <remarks>
    /// The API is designed to support high-volume invoice generation scenarios common in automotive parts trading.
    /// Bulk operations are provided for efficiency when processing multiple orders simultaneously.
    /// All endpoints include proper OpenAPI documentation and follow consistent REST patterns.
    /// </remarks>
    public static IEndpointRouteBuilder MapInvoicingEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Create a route group for all invoicing endpoints with consistent base path
        // This ensures proper API organization and OpenAPI documentation grouping
        var group = endpoints.MapGroup("/api/invoicing")
            .WithTags("Invoicing");

        // POST /api/invoicing/create - Create individual invoice for specific order or transaction
        // Returns 201 Created with invoice number - follows REST semantics for resource creation
        group.MapPost("/create", CreateInvoice)
            .WithName("CreateInvoice");

        // POST /api/invoicing/create-bulk - Create multiple invoices in a single operation
        // Returns 200 OK with collection of invoice numbers - optimized for high-volume scenarios
        group.MapPost("/create-bulk", CreateBulkInvoices)
            .WithName("CreateBulkInvoices");

        return endpoints;
    }

    /// <summary>
    /// Creates a single invoice based on order information and business requirements.
    /// This endpoint handles individual invoice generation with full tax calculations, regulatory compliance,
    /// and integration with accounting systems for standard business transactions.
    /// </summary>
    /// <param name="command">The command containing invoice details, order information, contractor data, and tax requirements</param>
    /// <param name="mediator">MediatR instance for executing the command through the application layer with full validation</param>
    /// <returns>HTTP 201 Created with invoice number and location header, enabling immediate invoice access</returns>
    /// <remarks>
    /// Invoice creation is a critical business operation that must comply with Polish and EU tax regulations.
    /// The operation includes comprehensive tax calculations, VAT processing, and regulatory compliance checks.
    /// The generated invoice number can be used immediately for document retrieval, printing, and customer communication.
    /// All invoices maintain complete audit trails for regulatory compliance and business reporting.
    /// </remarks>
    private static async Task<IResult> CreateInvoice(
        [FromBody] CreateInvoiceCommand command,
        IMediator mediator)
    {
        // Execute invoice creation with comprehensive business validation and tax calculations
        // Includes regulatory compliance checks and integration with accounting systems
        var result = await mediator.Send(command);

        // Return 201 Created with invoice number or 400 Bad Request on validation failure
        // The invoice number enables immediate access to the newly created invoice resource
        return result.IsSuccess
            ? Results.Created($"/api/invoicing/{result.Value}", new { InvoiceNumber = result.Value })
            : Results.BadRequest(result.Errors);
    }

    /// <summary>
    /// Creates multiple invoices simultaneously for efficient processing of large order volumes.
    /// This endpoint supports high-volume business scenarios where multiple orders need to be invoiced
    /// together, optimizing performance and reducing processing time for bulk operations.
    /// </summary>
    /// <param name="command">The command containing multiple invoice requests with order details, contractor information, and processing options</param>
    /// <param name="mediator">MediatR instance for executing the bulk command through optimized application layer processing</param>
    /// <returns>HTTP 200 OK with collection of generated invoice numbers and count summary for batch processing verification</returns>
    /// <remarks>
    /// Bulk invoice creation is essential for high-volume automotive parts trading operations.
    /// The operation is optimized for performance while maintaining full regulatory compliance and audit trails.
    /// Each invoice in the batch undergoes the same validation and tax calculation processes as individual invoices.
    /// The response includes both the invoice numbers and count for easy verification of batch processing results.
    /// Failed individual invoices within the batch are handled gracefully without affecting successful creations.
    /// </remarks>
    private static async Task<IResult> CreateBulkInvoices(
        [FromBody] CreateBulkInvoicesCommand command,
        IMediator mediator)
    {
        // Execute bulk invoice creation with optimized processing and comprehensive validation
        // Maintains individual invoice integrity while providing batch processing efficiency
        var result = await mediator.Send(command);

        // Return 200 OK with invoice numbers collection or 400 Bad Request on validation failure
        // Enables clients to verify successful processing and handle any partial failures
        return result.IsSuccess
            ? Results.Ok(new { InvoiceNumbers = result.Value, Count = result.Value.Count() })
            : Results.BadRequest(result.Errors);
    }
}

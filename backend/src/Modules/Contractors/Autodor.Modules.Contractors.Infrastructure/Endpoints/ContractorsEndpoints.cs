using Autodor.Modules.Contractors.Application.Commands.CreateContractor;
using Autodor.Modules.Contractors.Application.Commands.UpdateContractor;
using Autodor.Modules.Contractors.Application.Commands.DeleteContractor;
using Autodor.Modules.Contractors.Application.Queries.GetAllContractors;
using Autodor.Modules.Contractors.Application.Queries.GetContractor;
using Autodor.Modules.Contractors.Application.Queries.GetContractorByNIP;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Shared.Infrastructure.Models;

namespace Autodor.Modules.Contractors.Infrastructure.Endpoints;

/// <summary>
/// Provides HTTP endpoints for contractor management operations within the Autodor automotive parts trading system.
/// This class implements a complete REST API for managing business relationships with automotive parts suppliers,
/// customers, and service providers. All operations follow CQRS patterns and include comprehensive validation.
/// </summary>
/// <remarks>
/// The Contractors module manages critical business entities including suppliers, customers, and service providers.
/// These endpoints support the core business operations for partner relationship management, supplier onboarding,
/// customer management, and regulatory compliance (including NIP-based tax identification for Polish entities).
/// All endpoints implement proper HTTP semantics and return appropriate status codes for each business scenario.
/// </remarks>
public static class ContractorsEndpoints
{
    /// <summary>
    /// Configures and maps all HTTP endpoints for contractor management operations.
    /// This method establishes a complete REST API structure for contractor lifecycle management,
    /// including CRUD operations and specialized query endpoints for business-specific lookups.
    /// </summary>
    /// <param name="endpoints">The endpoint route builder used to configure API routes</param>
    /// <returns>The configured endpoint route builder for method chaining</returns>
    /// <remarks>
    /// The API follows RESTful conventions with proper HTTP verbs and resource-based routing.
    /// All endpoints support OpenAPI documentation and are grouped under "Contractors" for clear organization.
    /// Special endpoints like NIP-based lookup support regulatory and business requirements specific to Polish market.
    /// </remarks>
    public static IEndpointRouteBuilder MapContractorsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        // Create a route group for all contractor endpoints with consistent base path and tagging
        // This ensures API consistency and proper OpenAPI documentation grouping
        var group = endpoints.MapGroup("/api/contractors")
            .RequireAuthorization()
            .WithTags("Contractors");

        // POST /api/contractors - Create new contractor (supplier, customer, or service provider)
        // Returns 200 OK with generated ID - essential for business partner onboarding
        group.MapPost("/", CreateContractor)
            .WithName("CreateContractor")
            .WithOpenApi();

        // GET /api/contractors/{id} - Retrieve specific contractor by unique identifier
        // Returns 200 OK with contractor details - supports detailed business partner information display
        group.MapGet("/{id:guid}", GetContractor)
            .WithName("GetContractor")
            .WithOpenApi();

        // GET /api/contractors - Retrieve all contractors for listing and selection scenarios
        // Returns 200 OK with contractor collection - used in dropdowns and management interfaces
        group.MapGet("/", GetAllContractors)
            .WithName("GetAllContractors")
            .WithOpenApi();

        // PUT /api/contractors/{id} - Update existing contractor information
        // Returns 204 No Content on success - follows REST semantics for successful updates
        group.MapPut("/{id:guid}", UpdateContractor)
            .WithName("UpdateContractor")
            .WithOpenApi();

        // DELETE /api/contractors/{id} - Remove contractor from the system
        // Returns 204 No Content on success - supports cleanup and relationship termination
        group.MapDelete("/{id:guid}", DeleteContractor)
            .WithName("DeleteContractor")
            .WithOpenApi();

        // GET /api/contractors/by-nip/{nip} - Polish tax identification number lookup
        // Returns 200 OK with contractor details - critical for regulatory compliance and tax reporting
        group.MapGet("/by-nip/{nip}", GetContractorByNIP)
            .WithName("GetContractorByNIP")
            .WithOpenApi();

        return endpoints;
    }

    /// <summary>
    /// Creates a new contractor entity in the system, supporting business partner onboarding workflows.
    /// This endpoint handles the registration of suppliers, customers, and service providers with complete
    /// validation of business information, tax identification, and contact details.
    /// </summary>
    /// <param name="command">The command containing all contractor information including business details, contact info, and tax data</param>
    /// <param name="mediator">MediatR instance for executing the command through the application layer with full validation</param>
    /// <returns>HTTP 200 OK with the newly generated contractor ID for subsequent operations</returns>
    /// <remarks>
    /// Contractor creation is a critical business operation that establishes new business relationships.
    /// The operation includes comprehensive validation of business data, tax identification numbers,
    /// and contact information. The returned ID is essential for linking orders, invoices, and payments.
    /// </remarks>
    private static async Task<IResult> CreateContractor(
        [FromBody] CreateContractorCommand command,
        IMediator mediator)
    {
        // Execute contractor creation command with full business validation
        // This includes tax ID validation, contact information verification, and business rule enforcement
        var result = await mediator.Send(command);

        // Return 200 OK with the generated contractor ID or 400 Bad Request on validation failure
        // The ID is immediately usable for creating orders, invoices, and other business documents
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors);
    }

    /// <summary>
    /// Retrieves detailed information for a specific contractor by their unique identifier.
    /// This endpoint provides comprehensive contractor data including business details, contact information,
    /// tax identification, and relationship history for detailed business partner management.
    /// </summary>
    /// <param name="id">The unique identifier of the contractor to retrieve</param>
    /// <param name="mediator">MediatR instance for executing the query through the application layer</param>
    /// <returns>HTTP 200 OK with complete contractor details, or 404 Not Found if contractor doesn't exist</returns>
    /// <remarks>
    /// This endpoint is frequently used in business workflows for displaying contractor information,
    /// preparing business documents, and supporting customer service operations.
    /// The response includes all necessary data for order processing and invoice generation.
    /// </remarks>
    private static async Task<IResult> GetContractor(
        Guid id,
        IMediator mediator)
    {
        // Execute query to retrieve detailed contractor information
        // Includes business validation to ensure data access permissions are respected
        var result = await mediator.Send(new GetContractorQuery(id));

        // Return 200 OK with contractor details or 404 Not Found if contractor doesn't exist
        // The response includes all business-relevant contractor information
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(result.Errors);
    }

    /// <summary>
    /// Retrieves a complete list of all contractors in the system for selection and management purposes.
    /// This endpoint supports business scenarios like dropdown population, contractor selection in forms,
    /// and comprehensive contractor management interfaces.
    /// </summary>
    /// <param name="mediator">MediatR instance for executing the query through the application layer</param>
    /// <returns>HTTP 200 OK with a collection of all contractors, including summary information for each</returns>
    /// <remarks>
    /// This endpoint is commonly used in user interfaces for contractor selection during order creation,
    /// invoice generation, and administrative management tasks. The response is optimized for display purposes
    /// and may include summary data rather than complete contractor details for performance reasons.
    /// Consider implementing pagination for large contractor databases in production environments.
    /// </remarks>
    private static async Task<IResult> GetAllContractors(
        IMediator mediator)
    {
        // Execute query to retrieve all contractors with appropriate business filtering
        // May include summary data optimized for selection and listing scenarios
        var result = await mediator.Send(new GetAllContractorsQuery());

        // Return 200 OK with contractor collection or 400 Bad Request on validation failure
        // Response is optimized for UI consumption and contractor selection workflows
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.BadRequest(result.Errors);
    }

    /// <summary>
    /// Updates an existing contractor's information with new business details, contact information, or tax data.
    /// This endpoint supports business scenarios like address changes, contact updates, tax ID modifications,
    /// and other contractor information maintenance operations.
    /// </summary>
    /// <param name="id">The unique identifier of the contractor to update (must match the command ID)</param>
    /// <param name="command">The command containing updated contractor information and validation data</param>
    /// <param name="mediator">MediatR instance for executing the command through the application layer</param>
    /// <returns>HTTP 204 No Content on successful update, or 400 Bad Request if ID mismatch occurs</returns>
    /// <remarks>
    /// This endpoint implements proper REST semantics by requiring ID consistency between route and payload.
    /// Updates are comprehensive and may trigger business processes like tax validation or address verification.
    /// The operation maintains audit trails and supports compliance requirements for business partner data changes.
    /// </remarks>
    private static async Task<IResult> UpdateContractor(
        Guid id,
        [FromBody] UpdateContractorCommand command,
        IMediator mediator)
    {
        // Validate consistency between route parameter and command payload
        // This prevents accidental updates to wrong contractors and ensures data integrity
        if (id != command.Id)
        {
            return Results.BadRequest("Route ID does not match command ID");
        }

        // Execute update command with full business validation and audit logging
        // May trigger additional business processes like tax validation or compliance checks
        var result = await mediator.Send(command);

        // Return 204 No Content on success or 404 Not Found if contractor doesn't exist
        // This indicates successful processing without returning updated data
        return result.IsSuccess
            ? Results.NoContent()
            : Results.NotFound(result.Errors);
    }

    /// <summary>
    /// Removes a contractor from the system, handling business relationship termination and cleanup operations.
    /// This endpoint supports scenarios like supplier deactivation, customer account closure, and business
    /// relationship termination with appropriate validation and referential integrity checks.
    /// </summary>
    /// <param name="id">The unique identifier of the contractor to delete</param>
    /// <param name="mediator">MediatR instance for executing the command through the application layer</param>
    /// <returns>HTTP 204 No Content on successful deletion, with appropriate error responses for constraint violations</returns>
    /// <remarks>
    /// Contractor deletion is a sensitive business operation that may be restricted based on existing relationships.
    /// The operation includes validation for active orders, pending invoices, and other business constraints.
    /// Some implementations may perform soft deletion to maintain audit trails and historical data integrity.
    /// Business rules may prevent deletion of contractors with active business relationships.
    /// </remarks>
    private static async Task<IResult> DeleteContractor(
        Guid id,
        IMediator mediator)
    {
        // Execute deletion command with comprehensive business validation
        // Includes checks for active orders, pending invoices, and referential integrity constraints
        var result = await mediator.Send(new DeleteContractorCommand(id));

        // Return 204 No Content on success or 404 Not Found if contractor doesn't exist
        // Indicates successful processing - the resource no longer exists
        return result.IsSuccess
            ? Results.NoContent()
            : Results.NotFound(result.Errors);
    }

    /// <summary>
    /// Retrieves contractor information using the Polish tax identification number (NIP) for regulatory compliance.
    /// This endpoint supports tax reporting, regulatory compliance, and business verification workflows specific
    /// to Polish market requirements and European Union tax regulations.
    /// </summary>
    /// <param name="nip">The Polish tax identification number (NIP) to search for</param>
    /// <param name="mediator">MediatR instance for executing the query through the application layer</param>
    /// <returns>HTTP 200 OK with contractor details, or 404 Not Found if no contractor matches the NIP</returns>
    /// <remarks>
    /// NIP-based lookup is critical for Polish business operations and tax compliance requirements.
    /// This endpoint supports workflows like tax document generation, regulatory reporting, and business verification.
    /// The NIP format validation and business rules are enforced at the application layer.
    /// This functionality is essential for VAT processing and official business documentation.
    /// </remarks>
    private static async Task<IResult> GetContractorByNIP(
        string nip,
        IMediator mediator)
    {
        // Execute NIP-based contractor lookup with format validation and business rules
        // Critical for Polish tax compliance and regulatory reporting requirements
        var result = await mediator.Send(new GetContractorByNIPQuery(nip));

        // Return 200 OK with contractor details or 404 Not Found if no match found
        // Essential for tax document generation and regulatory compliance workflows
        return result.IsSuccess
            ? Results.Ok(result.Value)
            : Results.NotFound(result.Errors);
    }
}

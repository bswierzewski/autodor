using Autodor.Modules.Contractors.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Queries.GetAllContractors;

/// <summary>
/// Handles retrieval of all contractors in the automotive parts distribution system.
/// This query handler provides efficient bulk contractor data retrieval for management interfaces,
/// reporting systems, and business operations requiring complete contractor listings.
/// Uses read-only database context for optimal performance without change tracking overhead.
/// </summary>
public class GetAllContractorsQueryHandler : IRequestHandler<GetAllContractorsQuery, IEnumerable<GetAllContractorsDto>>
{
    private readonly IContractorsReadDbContext _readDbContext;

    /// <summary>
    /// Initializes a new instance of the GetAllContractorsQueryHandler.
    /// The read-only database context is injected to enable optimized bulk query operations.
    /// </summary>
    /// <param name="readDbContext">The read-only database context for efficient contractor data retrieval</param>
    public GetAllContractorsQueryHandler(IContractorsReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    /// <summary>
    /// Processes the request to retrieve all contractors in the system.
    /// This method performs a bulk database query to fetch all contractor entities and transforms
    /// them into client-friendly DTOs for efficient consumption by management interfaces.
    /// </summary>
    /// <param name="request">The query request (parameterless for retrieving all contractors)</param>
    /// <param name="cancellationToken">Token for cancelling the operation if needed</param>
    /// <returns>A collection of DTOs containing all contractors' business information</returns>
    public async Task<IEnumerable<GetAllContractorsDto>> Handle(GetAllContractorsQuery request, CancellationToken cancellationToken)
    {
        // Retrieve all contractors from the database in a single efficient query
        // Using ToListAsync to ensure the query executes and data is materialized
        var contractors = await _readDbContext.Contractors.ToListAsync(cancellationToken);

        // Transform domain entities into client-friendly DTOs
        // This projection extracts values from value objects for easy client consumption
        // Performed in memory after database fetch for simplicity and full data access
        return contractors.Select(contractor => new GetAllContractorsDto(
            contractor.Id.Value,           // Unique identifier
            contractor.Name,               // Business name
            contractor.NIP.Value,          // Tax identification number
            contractor.Address.Street,     // Street address
            contractor.Address.City,       // City location
            contractor.Address.ZipCode,    // Postal code
            contractor.Email.Value         // Email contact
        ));
    }
}
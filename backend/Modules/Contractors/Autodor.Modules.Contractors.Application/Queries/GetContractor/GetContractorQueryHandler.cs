using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractor;

/// <summary>
/// Handles retrieval of specific contractor information by unique identifier.
/// This query handler implements optimized read operations using the read-only database context,
/// providing efficient contractor lookup without the overhead of change tracking.
/// Essential for contractor management operations and business workflows requiring detailed contractor data.
/// </summary>
public class GetContractorQueryHandler : IRequestHandler<GetContractorQuery, GetContractorDto>
{
    private readonly IContractorsReadDbContext _readDbContext;

    /// <summary>
    /// Initializes a new instance of the GetContractorQueryHandler.
    /// The read-only database context is injected to enable optimized query operations without change tracking.
    /// </summary>
    /// <param name="readDbContext">The read-only database context for efficient contractor data retrieval</param>
    public GetContractorQueryHandler(IContractorsReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    /// <summary>
    /// Processes the contractor retrieval query by locating and returning the contractor data.
    /// This method performs efficient database lookup using the contractor's unique identifier
    /// and transforms the domain entity into a client-friendly DTO format.
    /// </summary>
    /// <param name="request">The query containing the contractor ID to retrieve</param>
    /// <param name="cancellationToken">Token for cancelling the operation if needed</param>
    /// <returns>A DTO containing the contractor's complete business information</returns>
    /// <exception cref="InvalidOperationException">Thrown when the contractor with the specified ID does not exist</exception>
    public async Task<GetContractorDto> Handle(GetContractorQuery request, CancellationToken cancellationToken)
    {
        // Locate the contractor using their unique identifier
        // Using the read context for optimized query performance without change tracking
        var contractor = await _readDbContext.Contractors
            .FirstOrDefaultAsync(c => c.Id == new ContractorId(request.Id), cancellationToken);

        // Validate that the contractor exists and provide clear error messaging
        // This ensures clients receive appropriate feedback for non-existent entities
        if (contractor is null)
            throw new InvalidOperationException($"Contractor with ID {request.Id} not found");

        // Transform the domain aggregate into a client-friendly DTO
        // This flattening process extracts values from value objects for easy client consumption
        return new GetContractorDto(
            contractor.Id.Value,           // Unique identifier
            contractor.Name,               // Business name
            contractor.NIP.Value,          // Tax identification number
            contractor.Address.Street,     // Street address
            contractor.Address.City,       // City location
            contractor.Address.ZipCode,    // Postal code
            contractor.Email.Value         // Email contact
        );
    }
}
using Autodor.Modules.Contractors.Domain.ValueObjects;
using Autodor.Modules.Contractors.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractorByNIP;

/// <summary>
/// Handles retrieval of contractor information using tax identification number (NIP) as the search key.
/// This query handler enables business-critical operations like invoice processing and tax compliance
/// by providing efficient contractor lookup using the legally-required tax identifier.
/// Essential for integrations with accounting systems and government tax reporting requirements.
/// </summary>
public class GetContractorByNIPQueryHandler : IRequestHandler<GetContractorByNIPQuery, GetContractorByNIPDto>
{
    private readonly IContractorsReadDbContext _readDbContext;

    /// <summary>
    /// Initializes a new instance of the GetContractorByNIPQueryHandler.
    /// The read-only database context is injected to enable optimized NIP-based contractor lookups.
    /// </summary>
    /// <param name="readDbContext">The read-only database context for efficient contractor data retrieval</param>
    public GetContractorByNIPQueryHandler(IContractorsReadDbContext readDbContext)
    {
        _readDbContext = readDbContext;
    }

    /// <summary>
    /// Processes the contractor retrieval query using NIP as the search criterion.
    /// This method performs tax-ID-based contractor lookup, essential for invoice processing,
    /// tax compliance, and business partner verification operations.
    /// </summary>
    /// <param name="request">The query containing the NIP (tax identification number) to search for</param>
    /// <param name="cancellationToken">Token for cancelling the operation if needed</param>
    /// <returns>A DTO containing the contractor's complete business information</returns>
    /// <exception cref="InvalidOperationException">Thrown when no contractor with the specified NIP exists</exception>
    public async Task<GetContractorByNIPDto> Handle(GetContractorByNIPQuery request, CancellationToken cancellationToken)
    {
        // Locate the contractor using their tax identification number (NIP)
        // This search is business-critical for tax compliance and invoice processing operations
        var contractor = await _readDbContext.Contractors
            .FirstOrDefaultAsync(x => x.NIP == new TaxId(request.NIP), cancellationToken);

        // Validate that a contractor exists with the provided NIP
        // This ensures compliance operations receive appropriate feedback for invalid tax IDs
        if (contractor is null)
            throw new InvalidOperationException($"Contractor with NIP {request.NIP} not found");

        // Transform the domain aggregate into a client-friendly DTO
        // This flattening provides all contractor details needed for tax and business operations
        return new GetContractorByNIPDto(
            contractor.Id.Value,           // Unique identifier
            contractor.Name,               // Business name
            contractor.NIP.Value,          // Tax identification number (validated)
            contractor.Address.Street,     // Street address
            contractor.Address.City,       // City location
            contractor.Address.ZipCode,    // Postal code
            contractor.Email.Value         // Email contact
        );
    }
}
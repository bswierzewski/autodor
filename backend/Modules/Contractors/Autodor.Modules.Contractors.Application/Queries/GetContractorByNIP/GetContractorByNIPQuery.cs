using MediatR;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractorByNIP;

/// <summary>
/// Query for retrieving a contractor by their tax identification number (NIP).
/// This query enables contractor lookup using the business-critical tax identifier,
/// essential for invoice processing, tax compliance operations, and business partner verification.
/// Particularly useful when integrating with external systems that reference contractors by NIP.
/// </summary>
/// <param name="NIP">The tax identification number (Polish NIP format) to search for</param>
public record GetContractorByNIPQuery(string NIP) : IRequest<GetContractorByNIPDto>;

/// <summary>
/// Data transfer object representing contractor information retrieved by tax identification number.
/// This DTO provides complete contractor details optimized for tax-related operations,
/// invoice processing, and business compliance workflows where NIP is the primary identifier.
/// Contains all essential business information to support downstream operations.
/// </summary>
/// <param name="Id">The unique identifier of the contractor</param>
/// <param name="Name">The business name used for identification and correspondence</param>
/// <param name="NIP">The tax identification number for legal compliance and business transactions</param>
/// <param name="Street">The street address for delivery and correspondence purposes</param>
/// <param name="City">The city location for regional operations and logistics</param>
/// <param name="ZipCode">The postal code for accurate mail delivery and geographic identification</param>
/// <param name="Email">The email address for electronic communication and business correspondence</param>
public record GetContractorByNIPDto(
    Guid Id,
    string Name,
    string NIP,
    string Street,
    string City,
    string ZipCode,
    string Email
);
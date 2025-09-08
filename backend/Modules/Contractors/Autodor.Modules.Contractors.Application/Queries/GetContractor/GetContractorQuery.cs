using MediatR;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractor;

/// <summary>
/// Query for retrieving a specific contractor by their unique identifier.
/// This query follows the CQRS pattern by providing a dedicated read operation that returns
/// contractor information in a format optimized for client consumption. Used when detailed
/// contractor information is needed for display, editing, or business operations.
/// </summary>
/// <param name="Id">The unique identifier of the contractor to retrieve</param>
public record GetContractorQuery(Guid Id) : IRequest<GetContractorDto>;

/// <summary>
/// Data transfer object representing a contractor's complete business information.
/// This DTO provides a flattened view of contractor data optimized for client consumption,
/// including all essential business details needed for contractor management and operations.
/// Used as the response format for contractor retrieval queries.
/// </summary>
/// <param name="Id">The unique identifier of the contractor</param>
/// <param name="Name">The business name used for identification and correspondence</param>
/// <param name="NIP">The tax identification number for legal compliance and business transactions</param>
/// <param name="Street">The street address for delivery and correspondence purposes</param>
/// <param name="City">The city location for regional operations and logistics</param>
/// <param name="ZipCode">The postal code for accurate mail delivery and geographic identification</param>
/// <param name="Email">The email address for electronic communication and business correspondence</param>
public record GetContractorDto(
    Guid Id,
    string Name,
    string NIP,
    string Street,
    string City,
    string ZipCode,
    string Email
);
using MediatR;

namespace Autodor.Modules.Contractors.Application.Queries.GetAllContractors;

/// <summary>
/// Query for retrieving all contractors in the automotive parts distribution system.
/// This query provides a complete list of business partners for management interfaces,
/// reporting dashboards, and business operations requiring contractor selection.
/// Uses the CQRS pattern for optimized read operations without pagination for simplicity.
/// Consider implementing pagination for large contractor datasets in production environments.
/// </summary>
public record GetAllContractorsQuery() : IRequest<IEnumerable<GetAllContractorsDto>>;

/// <summary>
/// Data transfer object representing essential contractor information for list displays.
/// This DTO provides all contractor details in a flattened format optimized for client consumption
/// in scenarios like dropdown lists, management grids, and contractor selection interfaces.
/// Contains complete business information to minimize additional API calls.
/// </summary>
/// <param name="Id">The unique identifier of the contractor</param>
/// <param name="Name">The business name used for identification and correspondence</param>
/// <param name="NIP">The tax identification number for legal compliance and business transactions</param>
/// <param name="Street">The street address for delivery and correspondence purposes</param>
/// <param name="City">The city location for regional operations and logistics</param>
/// <param name="ZipCode">The postal code for accurate mail delivery and geographic identification</param>
/// <param name="Email">The email address for electronic communication and business correspondence</param>
public record GetAllContractorsDto(
    Guid Id,
    string Name,
    string NIP,
    string Street,
    string City,
    string ZipCode,
    string Email
);
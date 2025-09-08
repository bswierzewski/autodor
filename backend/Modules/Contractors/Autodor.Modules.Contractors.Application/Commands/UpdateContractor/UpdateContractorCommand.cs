using MediatR;

namespace Autodor.Modules.Contractors.Application.Commands.UpdateContractor;

/// <summary>
/// Command for updating an existing contractor's business information in the automotive parts distribution system.
/// This command enables modification of contractor details while preserving the entity's identity and relationships.
/// Used when contractor information changes due to business updates, relocations, or contact information changes.
/// The command follows CQRS principles by representing a specific business intent to modify contractor data.
/// </summary>
/// <param name="Id">The unique identifier of the contractor to update. Must reference an existing contractor entity.</param>
/// <param name="NIP">The updated tax identification number (Polish NIP format) for legal compliance and business transactions.</param>
/// <param name="Name">The updated business name used for identification and legal documentation.</param>
/// <param name="Street">The updated street address including building number and street name for delivery purposes.</param>
/// <param name="City">The updated city where the contractor is located for regional operations and logistics.</param>
/// <param name="ZipCode">The updated postal code for accurate mail delivery and geographic identification.</param>
/// <param name="Email">The updated email address for electronic communication and business correspondence.</param>
public record UpdateContractorCommand(
    Guid Id,
    string NIP,
    string Name,
    string Street,
    string City,
    string ZipCode,
    string Email
) : IRequest;
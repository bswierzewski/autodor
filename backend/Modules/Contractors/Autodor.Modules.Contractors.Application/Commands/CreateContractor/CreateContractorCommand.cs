using MediatR;

namespace Autodor.Modules.Contractors.Application.Commands.CreateContractor;

/// <summary>
/// Command for creating a new contractor in the automotive parts distribution system.
/// This command encapsulates all the business information required to establish a new business relationship
/// with a contractor who will supply or purchase automotive parts. The command follows the CQRS pattern
/// by representing a specific business intent to create a contractor entity.
/// </summary>
/// <param name="Name">The business name of the contractor. Used for identification and legal documentation in business transactions.</param>
/// <param name="NIP">The tax identification number (Polish NIP format) required for legal compliance and VAT calculations.</param>
/// <param name="Street">The street address including building number and street name for delivery and correspondence purposes.</param>
/// <param name="City">The city where the contractor is located, essential for regional business operations and logistics.</param>
/// <param name="ZipCode">The postal code for accurate mail delivery and geographic region identification.</param>
/// <param name="Email">The email address for electronic communication, order notifications, and business correspondence.</param>
public record CreateContractorCommand(
    string Name,
    string NIP,
    string Street,
    string City,
    string ZipCode,
    string Email
) : IRequest<Guid>;
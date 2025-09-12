using MediatR;

namespace Autodor.Modules.Contractors.Application.Commands.CreateContractor;

/// <summary>
/// Command to create a new contractor with the provided details.
/// </summary>
/// <param name="Name">Contractor name.</param>
/// <param name="NIP">Tax identification number.</param>
/// <param name="Street">Street address.</param>
/// <param name="City">City name.</param>
/// <param name="ZipCode">Postal code.</param>
/// <param name="Email">Contact email address.</param>
public record CreateContractorCommand(
    string Name,
    string NIP,
    string Street,
    string City,
    string ZipCode,
    string Email
) : IRequest<Guid>;
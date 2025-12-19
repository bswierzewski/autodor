using MediatR;

namespace Autodor.Modules.Contractors.Application.Commands.UpdateContractor;

/// <summary>
/// Command to update an existing contractor's details.
/// </summary>
/// <param name="Id">Contractor unique identifier.</param>
/// <param name="NIP">Tax identification number.</param>
/// <param name="Name">Contractor name.</param>
/// <param name="Street">Street address.</param>
/// <param name="City">City name.</param>
/// <param name="ZipCode">Postal code.</param>
/// <param name="Email">Contact email address.</param>
public record UpdateContractorCommand(
    Guid Id,
    string NIP,
    string Name,
    string Street,
    string City,
    string ZipCode,
    string Email
) : IRequest<Unit>;
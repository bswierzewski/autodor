using MediatR;
using Shared.Infrastructure.Models;

namespace Autodor.Modules.Contractors.Application.Queries.GetAllContractors;

/// <summary>
/// Query to retrieve all contractors from the system.
/// </summary>
public record GetAllContractorsQuery() : IRequest<Result<IEnumerable<GetAllContractorsDto>>>;

/// <summary>
/// Data transfer object containing contractor information for display purposes.
/// </summary>
/// <param name="Id">Contractor unique identifier.</param>
/// <param name="Name">Contractor name.</param>
/// <param name="NIP">Tax identification number.</param>
/// <param name="Street">Street address.</param>
/// <param name="City">City name.</param>
/// <param name="ZipCode">Postal code.</param>
/// <param name="Email">Contact email address.</param>
public record GetAllContractorsDto(
    Guid Id,
    string Name,
    string NIP,
    string Street,
    string City,
    string ZipCode,
    string Email
);
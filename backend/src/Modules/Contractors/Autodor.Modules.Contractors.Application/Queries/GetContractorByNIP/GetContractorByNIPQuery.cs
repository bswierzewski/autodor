using MediatR;
using Shared.Infrastructure.Models;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractorByNIP;

/// <summary>
/// Query to retrieve a contractor by their tax identification number (NIP).
/// </summary>
/// <param name="NIP">Tax identification number to search for.</param>
public record GetContractorByNIPQuery(string NIP) : IRequest<Result<GetContractorByNIPDto>>;

/// <summary>
/// Data transfer object containing contractor information retrieved by NIP.
/// </summary>
/// <param name="Id">Contractor unique identifier.</param>
/// <param name="Name">Contractor name.</param>
/// <param name="NIP">Tax identification number.</param>
/// <param name="Street">Street address.</param>
/// <param name="City">City name.</param>
/// <param name="ZipCode">Postal code.</param>
/// <param name="Email">Contact email address.</param>
public record GetContractorByNIPDto(
    Guid Id,
    string Name,
    string NIP,
    string Street,
    string City,
    string ZipCode,
    string Email
);
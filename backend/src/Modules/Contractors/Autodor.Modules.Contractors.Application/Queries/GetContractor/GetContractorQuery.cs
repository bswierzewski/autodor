using MediatR;
using Shared.Infrastructure.Models;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractor;

/// <summary>
/// Query to retrieve a specific contractor by their unique identifier.
/// </summary>
/// <param name="Id">Contractor unique identifier.</param>
public record GetContractorQuery(Guid Id) : IRequest<Result<GetContractorDto>>;

/// <summary>
/// Data transfer object containing contractor information for a single contractor.
/// </summary>
/// <param name="Id">Contractor unique identifier.</param>
/// <param name="Name">Contractor name.</param>
/// <param name="NIP">Tax identification number.</param>
/// <param name="Street">Street address.</param>
/// <param name="City">City name.</param>
/// <param name="ZipCode">Postal code.</param>
/// <param name="Email">Contact email address.</param>
public record GetContractorDto(
    Guid Id,
    string Name,
    string NIP,
    string Street,
    string City,
    string ZipCode,
    string Email
);
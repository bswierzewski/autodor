using ErrorOr;
using MediatR;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractor;

public record GetContractorQuery(Guid? Id = null, string? NIP = null) : IRequest<ErrorOr<GetContractorDto>>;

public record GetContractorDto(
    Guid Id,
    string Name,
    string NIP,
    string Street,
    string City,
    string ZipCode,
    string Email
);
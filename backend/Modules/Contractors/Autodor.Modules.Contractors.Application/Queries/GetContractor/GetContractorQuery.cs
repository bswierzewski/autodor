using MediatR;

namespace Autodor.Modules.Contractors.Application.Queries.GetContractor;

public record GetContractorDto(
    Guid Id,
    string Name,
    string NIP,
    string Street,
    string City,
    string ZipCode,
    string Email
);

public record GetContractorQuery(Guid Id) : IRequest<GetContractorDto>;
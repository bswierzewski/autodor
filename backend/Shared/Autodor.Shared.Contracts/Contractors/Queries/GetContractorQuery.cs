using MediatR;

namespace Autodor.Shared.Contracts.Contractors.Queries;

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
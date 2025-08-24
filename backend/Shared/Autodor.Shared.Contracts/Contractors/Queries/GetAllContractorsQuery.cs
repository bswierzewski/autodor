using MediatR;

namespace Autodor.Shared.Contracts.Contractors.Queries;

public record GetAllContractorsDto(
    Guid Id,
    string Name,
    string NIP,
    string Street,
    string City,
    string ZipCode,
    string Email
);

public record GetAllContractorsQuery() : IRequest<IEnumerable<GetAllContractorsDto>>;